using AutoMapper;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyData.Model;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Exception;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.CommonFunction;
using HDNXUdemyServices.IServices;
using Microsoft.Extensions.Caching.Distributed;
using NodaTime;
using Stripe;
using Stripe.Checkout;

namespace HDNXUdemyServices.Services
{
    public class StripeServices : IStripeServices
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly IRCouponRepository _couponRepository;
        private readonly IRPromotionCodeRepository _promotionCodeRepository;
        private readonly IDistributedCache _distributedCache;

        public StripeServices(ICourseRepository courseRepository, IMapper mapper, IRCouponRepository couponRepository, IRPromotionCodeRepository promotionCodeRepository,
            IDistributedCache distributedCache)
        {
            _courseRepository = courseRepository ?? throw new ProjectException(nameof(_courseRepository));
            _mapper = mapper ?? throw new ProjectException(nameof(_mapper));
            _couponRepository = couponRepository ?? throw new ProjectException(nameof(_couponRepository));
            _promotionCodeRepository = promotionCodeRepository ?? throw new ProjectException(nameof(_promotionCodeRepository));
            _distributedCache = distributedCache ?? throw new ProjectException(nameof(_distributedCache));
        }

        public async Task<CheckoutSessionResponse> CreateCheckoutSession(PurcharseCourseModel model)
        {
            if (model.ListPurchaseCourseDetails == null || !model.ListPurchaseCourseDetails.Any())
            {
                throw new ProjectBadRequestException("No courses were provided for checkout.");
            }

            if (model.TotalPrice < 0 || model.ListPurchaseCourseDetails.Any(x => x.IdCourse <= 0 || x.PriceOfCourse < 0 || x.PriceOfDiscount < 0))
            {
                throw new ProjectBadRequestException("Checkout payload is invalid.");
            }

            var listSessionLineItemOption = new List<SessionLineItemOptions>();
            var returnValue = new CheckoutSessionResponse();
            foreach (var item in model.ListPurchaseCourseDetails)
            {
                var dataOfCoures = await _courseRepository.GetByIdAsync(item.IdCourse);
                if (dataOfCoures == null)
                {
                    throw new ProjectNotFoundException("Course not found.");
                }

                if (dataOfCoures.PriceOfCourse < 0)
                {
                    throw new ProjectBadRequestException("Course price is invalid.");
                }

                var itemSessionItemOption = new SessionLineItemOptions()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmountDecimal = dataOfCoures.PriceOfCourse,
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = dataOfCoures.Title,
                            Images = string.IsNullOrWhiteSpace(dataOfCoures.PictureUrl)
                                ? new List<string>()
                                : new List<string> { dataOfCoures.PictureUrl },
                        }
                    },
                };
                listSessionLineItemOption.Add(itemSessionItemOption);
            }

            if (string.IsNullOrWhiteSpace(ProjectConfig.StripeSecretKey) ||
                ProjectConfig.StripeSecretKey.StartsWith("CHANGE_ME", StringComparison.OrdinalIgnoreCase))
            {
                throw new ProjectAwsException("Payment provider is unavailable.");
            }

            var options = new SessionCreateOptions
            {
                SuccessUrl = ProjectConfig.StripePaymentSuccessLink,
                CancelUrl = ProjectConfig.StripePaymentUnSuccessLink,
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                Mode = "payment",
                LineItems = listSessionLineItemOption,
                ClientReferenceId = model.PurcharseCode.ToString(),
            };

            try
            {
                var stripeServices = new SessionService();
                var session = await stripeServices.CreateAsync(options);
                returnValue.SessionId = session.Id;
                await DistributedCacheRedis.SetDataToDistributedCache(_distributedCache, model.PurcharseCode.ToString(), model);
            }
            catch (StripeException ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
            catch (Exception ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
            return returnValue;
        }

        public async Task<bool> CreateStripePromotionCode(string idCoupon, string promotionCode)
        {
            if (string.IsNullOrWhiteSpace(idCoupon) || string.IsNullOrWhiteSpace(promotionCode))
            {
                throw new ProjectBadRequestException("Coupon and promotion code are required.");
            }

            if (promotionCode.Length > 100 || promotionCode.Contains("<script", StringComparison.OrdinalIgnoreCase))
            {
                throw new ProjectBadRequestException("Promotion code is invalid.");
            }

            var duplicatedCode = await _promotionCodeRepository.GetObjectAsync(x => x.StripePromotionCodeId == promotionCode);
            if (duplicatedCode != null)
            {
                throw new ProjectConflictException("Promotion code already exists.");
            }

            try
            {
                var options = new PromotionCodeCreateOptions
                {
                    Coupon = idCoupon,
                    Code = promotionCode,
                    Active = true,
                };
                var service = new PromotionCodeService();
                var stripeCreatePromotionCode = await service.CreateAsync(options);
                var mapperStripeToModel = _mapper.Map<PromotionCodeModel>(stripeCreatePromotionCode);
                var dataInsert = _mapper.Map<PromotionCodeEntities>(mapperStripeToModel);
                return await _promotionCodeRepository.AddAsync(dataInsert);
            }
            catch (StripeException ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
            catch (Exception ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
        }

        public async Task<bool> CreateCouponForPromotion(CouponPromotionCode model)
        {
            await ValidateCoupon(model, isUpdate: false);
            var options = new CouponCreateOptions
            {
                Duration = model.TypeDuration,
                PercentOff = model.PercentOff,
                Currency = model.Currency,
            };
            try
            {
                var dataInsert = new CouponEntities();
                if (model.Status == (int)EStatus.Active)
                {
                    var service = new CouponService();
                    var stripeCreateCoupon = await service.CreateAsync(options);
                    var mapperStripeToModel = _mapper.Map<CouponModel>(stripeCreateCoupon);
                    mapperStripeToModel.NameOfCoupon = model.NameOfCoupon;
                    mapperStripeToModel.StartDate = LocalDateTime.FromDateTime(model.StartDate ?? DateTime.UtcNow);
                    mapperStripeToModel.EndDate = LocalDateTime.FromDateTime(model.EndDate ?? DateTime.UtcNow);
                    mapperStripeToModel.Status = model.Status;
                    dataInsert = _mapper.Map<CouponEntities>(MappingCouponModel(mapperStripeToModel, model));
                }
                dataInsert = _mapper.Map<CouponEntities>(MappingCouponModel(new CouponModel(), model));

                return await _couponRepository.AddAsync(dataInsert);
            }
            catch (StripeException ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
            catch (Exception ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
        }

        public async Task<bool> DeleteCouponForPromotion(string stripeCouponId)
        {
            if (string.IsNullOrWhiteSpace(stripeCouponId) || stripeCouponId.Contains("..") || stripeCouponId.Contains("<script", StringComparison.OrdinalIgnoreCase))
            {
                throw new ProjectBadRequestException("Coupon id is invalid.");
            }

            try
            {
                var service = new CouponService();
                var deleteCouponStripe = await service.DeleteAsync(stripeCouponId);
                var getCouponOnData = await _couponRepository.GetObjectAsync(x => x.StripeCouponId == stripeCouponId);
                if (getCouponOnData == null)
                {
                    return deleteCouponStripe.Deleted ?? false;
                }

                getCouponOnData.Status = (int)EStatus.Inactive;
                return await _couponRepository.UpdateAsync(getCouponOnData);
            }
            catch (StripeException ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
            catch (Exception ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
        }

        public async Task<bool> InactivePromotionCode(string promotionCodeId)
        {
            if (string.IsNullOrWhiteSpace(promotionCodeId) || promotionCodeId.Contains("..") || promotionCodeId.Contains("<script", StringComparison.OrdinalIgnoreCase))
            {
                throw new ProjectBadRequestException("Promotion code id is invalid.");
            }

            try
            {
                var option = new PromotionCodeUpdateOptions()
                {
                    Active = false,
                };
                var service = new PromotionCodeService();
                var dataReturn = await service.UpdateAsync(promotionCodeId, option);
                return dataReturn.Id != string.Empty;
            }
            catch (StripeException ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
            catch (Exception ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
        }

        public async Task<PagedResult<CouponModel>> GetListCouponActiveOnSystem(int pageIndex, int pageSize)
        {
            ValidatePaging(pageIndex, pageSize);
            var getDataFromAPI = (await _couponRepository.GetAsync(x => x.Status != (int)EStatus.Inactive))
                .GetPagingPaged(pageIndex, pageSize, null);
            return _mapper.Map<PagedResult<CouponModel>>(getDataFromAPI);
        }

        public async Task<bool> UpdateCouponPromotionCode(CouponPromotionCode model)
        {
            await ValidateCoupon(model, isUpdate: true);
            var entitiesCoupon = await _couponRepository.GetByIdAsync(model.Id ?? 0);
            if (entitiesCoupon == null) { throw new ProjectNotFoundException("Coupon not found."); }
            entitiesCoupon.StartDate = LocalDateTime.FromDateTime(model.StartDate ?? DateTime.UtcNow);
            entitiesCoupon.EndDate = LocalDateTime.FromDateTime(model.EndDate ?? DateTime.UtcNow);
            entitiesCoupon.Status = model.Status;
            entitiesCoupon.NameOfCoupon = model.NameOfCoupon;
            entitiesCoupon.PercentOff = model.PercentOff;

            if (model.Status == (int)EStatus.Active && entitiesCoupon.StripeCouponId == null)
            {
                return await ActiveCouponPromotion(entitiesCoupon);
            }
            else
            {
                return await _couponRepository.UpdateAsync(entitiesCoupon);
            }
        }

        public async Task<PagedResult<PromotionCodeModel>> GetListPromotions(int pageIndex, int pageSize)
        {
            ValidatePaging(pageIndex, pageSize);
            var getDataFromAPI = (await _promotionCodeRepository.GetAsync(x => x.Status != (int)EStatus.Inactive))
                .GetPagingPaged(pageIndex, pageSize, null);
            return _mapper.Map<PagedResult<PromotionCodeModel>>(getDataFromAPI);

        }

        private async Task<bool> ActiveCouponPromotion(CouponEntities entities)
        {
            try
            {
                var options = new CouponCreateOptions
                {
                    Duration = entities.Duration,
                    PercentOff = entities.PercentOff,
                    Currency = entities.Currency
                };
                var service = new CouponService();
                var stripeCreateCoupon = await service.CreateAsync(options);
                var mapperStripeToModel = _mapper.Map<CouponEntities>(stripeCreateCoupon) ?? new CouponEntities();
                mapperStripeToModel.Status = (int)EStatus.Active;
                mapperStripeToModel.Id = entities.Id;
                mapperStripeToModel.CreateDate = entities.CreateDate;
                mapperStripeToModel.NameOfCoupon = entities.NameOfCoupon;
                mapperStripeToModel.StartDate = entities.StartDate;
                mapperStripeToModel.EndDate = entities.EndDate;
                return await _couponRepository.UpdateAsync(mapperStripeToModel);
            }
            catch (StripeException ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
            catch (Exception ex)
            {
                throw new ProjectAwsException("Payment provider is unavailable.", ex);
            }
        }

        private CouponModel MappingCouponModel(CouponModel modelReturn,  CouponPromotionCode model)
        {
            modelReturn.NameOfCoupon = model.NameOfCoupon;
            modelReturn.Duration = model.TypeDuration;
            modelReturn.Currency = model.Currency;
            modelReturn.PercentOff = model.PercentOff;
            modelReturn.StartDate = LocalDateTime.FromDateTime(model.StartDate ?? DateTime.UtcNow);
            modelReturn.EndDate = LocalDateTime.FromDateTime(model.EndDate ?? DateTime.UtcNow);
            modelReturn.Status = model.Status;
            return modelReturn;
        }

        private async Task ValidateCoupon(CouponPromotionCode model, bool isUpdate)
        {
            if (model == null)
            {
                throw new ProjectBadRequestException("Payload is required.");
            }

            if (isUpdate && (!model.Id.HasValue || model.Id.Value <= 0))
            {
                throw new ProjectBadRequestException("Id is invalid.");
            }

            if (string.IsNullOrWhiteSpace(model.NameOfCoupon))
            {
                throw new ProjectBadRequestException("Coupon name is required.");
            }

            if (model.NameOfCoupon.Length > 150 || model.NameOfCoupon.Contains("<script", StringComparison.OrdinalIgnoreCase))
            {
                throw new ProjectBadRequestException("Coupon name is invalid.");
            }

            if (model.PercentOff <= 0 || model.PercentOff > 100)
            {
                throw new ProjectBadRequestException("PercentOff must be between 1 and 100.");
            }

            if (string.IsNullOrWhiteSpace(model.Currency) || model.Currency.Length != 3)
            {
                throw new ProjectBadRequestException("Currency is invalid.");
            }

            var allowedDurations = new[] { "forever", "once", "repeating" };
            if (string.IsNullOrWhiteSpace(model.TypeDuration) || !allowedDurations.Contains(model.TypeDuration, StringComparer.OrdinalIgnoreCase))
            {
                throw new ProjectBadRequestException("Duration is invalid.");
            }

            if (!model.StartDate.HasValue || !model.EndDate.HasValue || model.StartDate.Value >= model.EndDate.Value)
            {
                throw new ProjectBadRequestException("Date range is invalid.");
            }

            if (!Enum.IsDefined(typeof(EStatus), model.Status))
            {
                throw new ProjectBadRequestException("Status is invalid.");
            }

            var duplicate = await _couponRepository.GetObjectAsync(x =>
                x.NameOfCoupon == model.NameOfCoupon && (!isUpdate || x.Id != model.Id));
            if (duplicate != null)
            {
                throw new ProjectConflictException("Coupon already exists.");
            }
        }

        private static void ValidatePaging(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1 || pageSize > 100)
            {
                throw new ProjectBadRequestException("Paging is invalid.");
            }
        }
    }
}
