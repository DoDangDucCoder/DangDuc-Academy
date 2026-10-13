import { Component, ViewChild } from '@angular/core';

// Get Modal
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { Observable } from 'rxjs';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Store } from '@ngrx/store';
// import ticket.actions
// import ticket-selector
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { cloneDeep } from 'lodash';
import { assignesTickets } from 'src/app/core/data';
import { StripeServices } from 'src/app/core/services/stripe.service';
import { PromotionCodeModel } from 'src/app/models/models/promotion-code';
import { PageResult } from 'src/app/models/models/repository_base';
import { CouponModel } from 'src/app/models/models/coupon';

@Component({
  selector: 'app-list-promotion',
  templateUrl: './list-promotion.component.html',
  styleUrls: ['./list-promotion.component.scss'],
  providers: [DecimalPipe]
})

// List component
export class ListPromotionsComponent {

  // bread crumb items
  breadCrumbItems!: Array<{}>;
  deleteId: string;
  endItem: any
  listForm!: UntypedFormGroup;
  submitted = false;
  masterSelected!: boolean;
  supportList: any;
  promotions: PageResult<PromotionCodeModel[]> = {
    currentPage: 1,
    pageCount: 0,
    pageSize: 10,
    rowCount: 0,
    firstRowOnPage: 0,
    lastRowOnPage: 0,
    results: []
  };
  // assigndata: any
  assignList: any;
  term: any
  @ViewChild('addPromotion', { static: false }) addPromotion?: ModalDirective;
  @ViewChild('deleteRecordModal', { static: false }) deleteRecordModal?: ModalDirective;
  assignto: any = [];
  editData: any;
  alltickets: any;
  isLoading = false;
  coupons: PageResult<CouponModel[]> = {
    currentPage: 1,
    pageCount: 0,
    pageSize: 10,
    rowCount: 0,
    firstRowOnPage: 0,
    lastRowOnPage: 0,
    results: []
  };

  constructor(
    private formBuilder: UntypedFormBuilder, 
    public store: Store, 
    public datepipe: DatePipe,
    private readonly stripeServices : StripeServices) {
  }

  ngOnInit(): void {
    /**
     * BreadCrumb
     */
    this.breadCrumbItems = [
      { label: 'Home', active: true },
      { label: 'List Promotions', active: true }
    ];

    /**
     * Form Validation
     */
    this.listForm = this.formBuilder.group({
      id: [''],
      promotionCode: ['', [Validators.required]],
      idCoupon: ['', [Validators.required]],
      startDate: ['', [Validators.required]],
      endDate: ['', [Validators.required]],
      status: ['', [Validators.required]]
    });
    this.assignList = assignesTickets
    this.loadDataOfPromotion(1, 10);
    this.loadDataOfCoupon(1, 100);
  }

  // Edit Data
  editList(id: any) {
    this.addPromotion?.show()
    var modaltitle = document.querySelector('.modal-title') as HTMLAreaElement
    modaltitle .textContent = 'Edit Promotion Code'
    var modalBtn = document.getElementById('add-btn') as HTMLAreaElement
    modalBtn .textContent = 'Update';
    this.editData = this.promotions.results?.[id];
    if (!this.editData) {
      return;
    }
    this.assignto = this.editData.assignedto ?? [];
    this.listForm.patchValue(this.editData);
  }

  // Add Assigne
  addAssign(id: any) {
    if (this.assignList[id].checked == '0') {
      this.assignList[id].checked = '1'
    } else {
      this.assignList[id].checked = '0'
    }

    this.assignto = [];
    this.assignList.forEach((element: any) => {
      if (element.checked == '1') {
        this.assignto.push(element)
      }
    });
  }

  // add Product
  saveList() {
    this.submitted = true
    if (this.listForm.valid) {
      if (this.listForm.get('id')?.value) {
        const updatedData = { assignedto: this.assignto, ...this.listForm.value };
        // this.store.dispatch(updateticketlistData({ updatedData }));
      }
      else {
        this.listForm.controls['id'].setValue((this.alltickets.length + 1).toString());
        const createDate = this.datepipe.transform(this.listForm.get('createDate')?.value, "dd MMM, yyyy") || ''
        const dueDate = this.datepipe.transform(this.listForm.get('dueDate')?.value, "dd MMM, yyyy") || ''
        this.listForm.patchValue({ createDate: createDate, dueDate: dueDate });

        const newData = { assignedto: this.assignto, ...this.listForm.value }
        // this.store.dispatch(addticketlistData({ newData }));
      }
      this.assignto = [];
      this.listForm.reset();
      this.addPromotion?.hide()
    }
  }

  checkedValGet: any[] = [];

  // Delete Product
  removeItem(id: string) {
    this.deleteId = id
    this.deleteRecordModal?.show()
  }

  deleteData(id: string) {
    this.deleteRecordModal?.hide();
    if (id) {
      // this.store.dispatch(deleteticketlistData({ id: this.deleteId.toString() }));
    }
    // this.store.dispatch(deleteticketlistData({ id: this.checkedValGet.toString() }));
    this.deleteRecordModal?.hide();
    this.masterSelected = false
  }
  // Sort Data
  direction: any = 'asc';
  onSort(column: any) {
    if (this.direction == 'asc') {
      this.direction = 'desc';
    } else {
      this.direction = 'asc';
    }
    const sortedArray = [...(this.promotions.results ?? [])]; // Create a new array
    sortedArray.sort((a, b) => {
      const res = this.compare(a[column], b[column]);
      return this.direction === 'asc' ? res : -res;
    });
    this.promotions.results = sortedArray;
  }
  compare(v1: string | number, v2: string | number) {
    return v1 < v2 ? -1 : v1 > v2 ? 1 : 0;
  }


  // filterdata
  filterdata() {
    if (this.term) {
      const source = this.alltickets?.results ?? this.promotions.results ?? [];
      this.promotions = { ...this.promotions, results: source.filter((es: any) => `${es.stripePromotionCodeId ?? ''}`.toLowerCase().includes(this.term.toLowerCase())) };
    } else {
      this.promotions = this.alltickets ?? this.promotions;
    }
    // noResultElement
    this.updateNoResultDisplay();
  }

  // no result 
  updateNoResultDisplay() {
    const noResultElement = document.querySelector('.noresult') as HTMLElement;
    const paginationElement = document.getElementById('pagination-element') as HTMLElement;

    if (this.term && (this.promotions.results?.length ?? 0) === 0) {
      noResultElement.classList.remove('d-none')
      paginationElement.classList.add('d-none')

    } else {
      noResultElement.classList.add('d-none')
      paginationElement.classList.remove('d-none')
    }
  }

  // pagechanged
  pageChanged(event: PageChangedEvent): void {
    const startItem = (event.page - 1) * event.itemsPerPage;
    this.endItem = event.page * event.itemsPerPage;
    const source = this.alltickets?.results ?? [];
    this.promotions = { ...this.promotions, results: source.slice(startItem, this.endItem) };
  }

  loadDataOfPromotion(pageIndex: number, pageSize: number) {
    this.isLoading = true;
    this.stripeServices.getListPromotions(pageIndex, pageSize).subscribe((res) => {
      if (res.systemMessage === '' && res.retCode === 0 && res.data) {
        this.isLoading = false;
        this.promotions = { ...this.promotions, ...res.data, results: res.data.results ?? [] };
        this.alltickets = this.promotions;
      } else {
        this.isLoading = false;
        this.promotions = { ...this.promotions, results: [] };
      }
    }, () => {
      this.isLoading = false;
      this.promotions = { ...this.promotions, results: [] };
    });
  }

  loadDataOfCoupon(pageIndex: number, pageSize: number) {
    this.isLoading = true;
    this.stripeServices.getListCouponAndPromotion(pageIndex, pageSize).subscribe((res) => {
      if (res.systemMessage === '' && res.retCode === 0 && res.data) {
        this.isLoading = false;
        this.coupons = { ...this.coupons, ...res.data, results: res.data.results ?? [] };
      } else {
        this.isLoading = false;
        this.coupons = { ...this.coupons, results: [] };
      }
    }, () => {
      this.isLoading = false;
      this.coupons = { ...this.coupons, results: [] };
    });
  }
}


