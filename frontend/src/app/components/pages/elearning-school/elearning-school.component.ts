import { Component, OnInit, OnDestroy } from '@angular/core';
import { HomeServices } from 'src/app/core/services/home.service';
import { CourseServices } from 'src/app/core/services/course.service';
import { HomeDataModel } from 'src/app/models/respone_model/home-data';

@Component({
    selector: 'app-elearning-school',
    templateUrl: './elearning-school.component.html',
    styleUrls: ['./elearning-school.component.scss']
})
export class ElearningSchoolComponent implements OnInit, OnDestroy {

    constructor(
        private readonly homeServices: HomeServices,
        private readonly courseServices: CourseServices) { }

    ngOnInit(): void {
        this.loadDataOfElearning();
        this.loadTrendingCourses();
        this.startCountdown();
    }

    homeDataOfProject: HomeDataModel | null | undefined;
    isLoading: boolean = false;
    trendingCourses: any[] = [];

    // Flash Sale
    showFlashSale = true;
    countdownHours = 0;
    countdownMinutes = 0;
    countdownSeconds = 0;
    private countdownInterval: any;

    loadDataOfElearning() {
        this.isLoading = true;
        this.homeServices.getHomeData().subscribe((res) => {
            if (res.retCode === 0 && res.systemMessage === '') {
                this.homeDataOfProject = res.data;
                this.isLoading = false;
            } else {
                this.isLoading = false;
            }
        });
    }

    loadTrendingCourses() {
        this.courseServices.getListAllCourse().subscribe((res: any) => {
            if (res.retCode === 0 && res.data) {
                // Get courses sorted by student count (trending)
                this.trendingCourses = (res.data as any[])
                    .sort((a: any, b: any) => (b.totalStudentRegister || 0) - (a.totalStudentRegister || 0))
                    .slice(0, 8);
            }
        });
    }

    startCountdown() {
        // Set sale end time to end of today
        const now = new Date();
        const endOfDay = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 23, 59, 59);
        this.countdownInterval = setInterval(() => {
            const diff = endOfDay.getTime() - new Date().getTime();
            if (diff <= 0) {
                this.showFlashSale = false;
                clearInterval(this.countdownInterval);
                return;
            }
            this.countdownHours = Math.floor(diff / (1000 * 60 * 60));
            this.countdownMinutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
            this.countdownSeconds = Math.floor((diff % (1000 * 60)) / 1000);
        }, 1000);
    }

    ngOnDestroy() {
        if (this.countdownInterval) {
            clearInterval(this.countdownInterval);
        }
    }

}