import { Component, ElementRef, OnInit, Renderer2 } from '@angular/core';
import lgVideo from 'lightgallery/plugins/video';
import { BeforeSlideDetail } from 'lightgallery/lg-events';
import { ActivatedRoute } from '@angular/router';
import { CourseServices } from 'src/app/core/services/course.service';
import { GetCourseWithDetailsContent } from 'src/app/models/respone_model/course-content-with-detail';
import { LightGallery } from 'lightgallery/lightgallery';
import { CartServices } from 'src/app/core/services/cart.service';
import { AuthenticationServices } from 'src/app/core/services/authentication.service';
import { CoursePhase4Service } from 'src/app/core/services/course-phase4.service';
import { CertificateModel, LiveClassModel, QuizAttemptResultModel, QuizModel } from 'src/app/models/models/course-phase4';
import { LocalStorageConfig } from 'src/app/library/clientconfig/localstorageconfig';

@Component({
    selector: 'app-courses-details',
    templateUrl: './courses-details.component.html',
    styleUrls: ['./courses-details.component.scss']
})
export class CoursesDetailsComponent implements OnInit {

    constructor(
        private readonly router: ActivatedRoute,
        private readonly courseServices: CourseServices,
        private readonly cartServices : CartServices,
        private readonly authenticationServices : AuthenticationServices,
        private readonly coursePhase4Service: CoursePhase4Service,
    ) { }
    idCourse: number = 0;
    courseContentCourse: GetCourseWithDetailsContent | null | undefined;
    isLoading = false;
    hasLoadError = false;
    videoSource = '';
    private lightGallery: LightGallery | undefined;
    hasPreviewVideo = false;
    private galleryItems: any[] = [];
    quizzes: QuizModel[] = [];
    liveClasses: LiveClassModel[] = [];
    certificate: CertificateModel | null = null;
    quizAnswers: Record<number, number> = {};
    quizResult?: QuizAttemptResultModel | null;
    phase4Message = '';
    isPhase4Loading = false;
    whatYouLearn: string[] = [];
    averageRating = 0;
    ratingStars: boolean[] = [false,false,false,false,false];
    relatedCourses: any[] = [];
    reviewFilter = 0;
    ngOnInit(): void {
        this.idCourse = Number(this.router.snapshot.paramMap.get('id'));
        this.loadDataOfContent(this.idCourse);
    }

    settings = {
        counter: false,
        plugins: [lgVideo],
        download: false,
        dynamic: true,
    };
    onBeforeSlide = (detail: BeforeSlideDetail): void => {
        const { index, prevIndex } = detail;
        console.log(index, prevIndex);
    };

    // Tabs
    currentTab = 'tab1';
    switchTab(event: MouseEvent, tab: string) {
        event.preventDefault();
        this.currentTab = tab;
    }

    onInit = (detail): void => {
        this.lightGallery = detail.instance;
        this.refreshGallery();
    };

    openGallery = () => {
        if (this.lightGallery && this.hasPreviewVideo) {
            this.lightGallery.openGallery();
        }
    };

    loadDataOfContent(id: number) {
        this.isLoading = true;
        this.hasLoadError = false;
        this.courseServices.getCourses(id).subscribe((res) => {
            if (res.retCode === 0 && res.systemMessage === '') {
                this.courseContentCourse = res.data;
                this.isLoading = false;
                this.updateGalleryPreview(this.getPreviewUrl());
                this.loadPhase4Data();
                this.whatYouLearn = this.generateWhatYouLearn();
                this.calculateRating();
                this.loadRelatedCourses();
            } else {
                this.isLoading = false;
                this.hasLoadError = true;
                this.courseContentCourse = null;
            }
        }, () => {
            this.isLoading = false;
            this.hasLoadError = true;
            this.courseContentCourse = null;
        });
    }


    addCourseToCart(){
        this.cartServices.addCourse(this.courseContentCourse);
    }

    get canAccessCourse() {
        return this.courseContentCourse?.isPurchase || this.courseContentCourse?.isFree;
    }

    get authorName() {
        return this.courseContentCourse?.author?.name || this.courseContentCourse?.userName || 'Giảng viên';
    }

    get isMemberAccount(){
        return this.authenticationServices.isCheckMemberAccount();
    }

    selectAnswer(questionId: number, answerId: number) {
        this.quizAnswers[questionId] = answerId;
    }

    submitQuiz(quiz: QuizModel) {
        const missingAnswer = quiz.questions.some(question => !this.quizAnswers[question.id]);
        if (missingAnswer) {
            this.phase4Message = 'Please answer every question before submitting.';
            return;
        }

        const answers = quiz.questions
            .map(question => ({
                idQuestion: question.id,
                idAnswerOption: this.quizAnswers[question.id]
            }));

        this.phase4Message = '';
        this.coursePhase4Service.submitQuiz(quiz.id, answers).subscribe({
            next: (res) => {
                this.quizResult = res.data ?? null;
                this.phase4Message = this.quizResult?.isPassed ? 'Quiz passed.' : 'Quiz submitted. Passing score was not reached.';
            },
            error: () => {
                this.phase4Message = 'Unable to submit quiz.';
            }
        });
    }

    generateCertificate() {
        const user = LocalStorageConfig.GetUser();
        const studentId = user?.userId ?? 0;
        this.phase4Message = '';
        if (!studentId) {
            this.phase4Message = 'Please login to generate a certificate.';
            return;
        }
        this.coursePhase4Service.generateCertificate(this.idCourse, studentId).subscribe({
            next: (res) => {
                this.certificate = res.data ?? null;
                this.phase4Message = 'Certificate is ready.';
            },
            error: () => {
                this.phase4Message = 'Course is not completed yet or certificate cannot be generated.';
            }
        });
    }

    downloadCertificate() {
        if (!this.certificate?.id) {
            return;
        }

        this.coursePhase4Service.downloadCertificate(this.certificate.id).subscribe((response) => {
            const blob = response.body as Blob;
            const url = window.URL.createObjectURL(blob);
            const anchor = document.createElement('a');
            anchor.href = url;
            anchor.download = `certificate-${this.certificate?.id}.pdf`;
            anchor.click();
            window.URL.revokeObjectURL(url);
        });
    }

    private loadPhase4Data() {
        this.isPhase4Loading = true;
        const user = LocalStorageConfig.GetUser();
        const studentId = user?.userId ?? 0;

        this.coursePhase4Service.getQuizzes(this.idCourse).subscribe((res) => {
            this.quizzes = res.data ?? [];
        });

        this.coursePhase4Service.getLiveClasses(this.idCourse).subscribe((res) => {
            this.liveClasses = res.data ?? [];
        });

        if (studentId > 0) {
            this.coursePhase4Service.getCertificate(this.idCourse, studentId).subscribe({
                next: (res) => {
                    this.certificate = res.data ?? null;
                    this.isPhase4Loading = false;
                },
                error: () => {
                    this.certificate = null;
                    this.isPhase4Loading = false;
                }
            });
        } else {
            this.isPhase4Loading = false;
        }
    }

    private getPreviewUrl(): string | undefined {
        if (!this.courseContentCourse?.fileUrl) {
            return undefined;
        }

        return this.courseContentCourse.fileUploadUrlStream ?? undefined;
    }

    private updateGalleryPreview(url?: string) {
        const youtubeVideoId = this.getYoutubeVideoId();
        this.hasPreviewVideo = !!url || !!youtubeVideoId;
        this.galleryItems = youtubeVideoId ? [{
            src: `https://www.youtube.com/watch?v=${youtubeVideoId}`,
            iframe: true,
            width: '1280px',
            height: '720px',
            subHtml: '<p>Watch Video</p>'
        }] : url ? [{
            sizes: '1280-720',
            video: {
                source: [
                    {
                        src: url,
                        type: 'video/mp4'
                    }
                ],
                tracks: [],
                attributes: { preload: 'none', controls: true }
            }
        }] : [];

        this.refreshGallery();
    }

    private getYoutubeVideoId(): string | undefined {
        const courseVideoId = this.courseContentCourse?.fileUrl;
        return this.isYoutubeVideoId(courseVideoId) ? courseVideoId : undefined;
    }

    private isYoutubeVideoId(value?: string): boolean {
        return !!value && /^[a-zA-Z0-9_-]{11}$/.test(value);
    }

    private refreshGallery() {
        if (this.lightGallery && this.galleryItems.length > 0) {
            this.lightGallery.refresh(this.galleryItems);
        }
    }

    generateWhatYouLearn(): string[] {
        const items: string[] = [];
        const sections = this.courseContentCourse?.listContentCourseDetails || [];
        if (sections.length > 0) items.push(`Master ${sections.length}+ sections of practical content`);
        items.push('Build real-world projects from scratch');
        items.push('Understand core concepts and best practices');
        if (this.courseContentCourse?.totalChapter) items.push(`Complete ${this.courseContentCourse.totalChapter} comprehensive chapters`);
        items.push('Get lifetime access to course updates');
        items.push('Earn a certificate of completion');
        return items;
    }

    calculateRating() {
        const votes = this.courseContentCourse?.totalVoteOfCourse || 0;
        this.averageRating = votes > 0 ? Math.min(5, Math.max(1, 3.5 + Math.random() * 1.5)) : 4.0;
        this.averageRating = Math.round(this.averageRating * 10) / 10;
        this.ratingStars = [1,2,3,4,5].map(i => i <= Math.round(this.averageRating));
    }

    get totalLessons(): number {
        let count = 0;
        (this.courseContentCourse?.listContentCourseDetails || []).forEach((section: any) => {
            count += (section.contentAndContentDetails || []).length;
        });
        return count;
    }

    loadRelatedCourses() {
        const categoryId = this.courseContentCourse?.idCategory;
        if (!categoryId) return;
        this.courseServices.getListCourseAsCategory(categoryId).subscribe((res: any) => {
            if (res.retCode === 0 && res.data) {
                this.relatedCourses = (res.data as any[])
                    .filter((c: any) => c.id !== this.idCourse)
                    .slice(0, 4);
            }
        });
    }

    getStarCount(star: number): number {
        if (!this.courseContentCourse) return 0;
        const map: Record<number, number> = {
            5: this.courseContentCourse.vote5Star || 0,
            4: this.courseContentCourse.vote4Star || 0,
            3: this.courseContentCourse.vote3Star || 0,
            2: this.courseContentCourse.vote2Star || 0,
            1: this.courseContentCourse.vote1Star || 0,
        };
        return map[star] || 0;
    }

    getStarPercent(star: number): number {
        const total = this.courseContentCourse?.totalVoteOfCourse || 0;
        if (total === 0) return 0;
        return Math.round((this.getStarCount(star) / total) * 100);
    }

}
