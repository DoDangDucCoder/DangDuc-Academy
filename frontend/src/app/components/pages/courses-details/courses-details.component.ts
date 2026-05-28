import { Component, ElementRef, OnInit, Renderer2 } from '@angular/core';
import lgVideo from 'lightgallery/plugins/video';
import { BeforeSlideDetail } from 'lightgallery/lg-events';
import { ActivatedRoute } from '@angular/router';
import { CourseServices } from 'src/app/core/services/course.service';
import { GetCourseWithDetailsContent } from 'src/app/models/respone_model/course-content-with-detail';
import { LightGallery } from 'lightgallery/lightgallery';
import { CartServices } from 'src/app/core/services/cart.service';
import { AuthenticationServices } from 'src/app/core/services/authentication.service';

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
    ) { }
    idCourse: number = 0;
    courseContentCourse: GetCourseWithDetailsContent | null | undefined;
    isLoading = false;
    hasLoadError = false;
    videoSource = '';
    private lightGallery: LightGallery | undefined;
    hasPreviewVideo = false;
    private galleryItems: any[] = [];
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

    get isMemberAccount(){
        return this.authenticationServices.isCheckMemberAccount();
    }

    private getPreviewUrl(): string | undefined {
        if (!this.courseContentCourse?.fileUrl) {
            return undefined;
        }

        return this.courseContentCourse.fileUploadUrlStream ?? undefined;
    }

    private updateGalleryPreview(url?: string) {
        this.hasPreviewVideo = !!url;
        this.galleryItems = url ? [{
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

    private refreshGallery() {
        if (this.lightGallery && this.galleryItems.length > 0) {
            this.lightGallery.refresh(this.galleryItems);
        }
    }

}
