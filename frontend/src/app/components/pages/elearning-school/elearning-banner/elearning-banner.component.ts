import { Component, Input, OnInit } from '@angular/core';
import { Course } from 'src/app/models/models/course';

@Component({
    selector: 'app-elearning-banner',
    templateUrl: './elearning-banner.component.html',
    styleUrls: ['./elearning-banner.component.scss']
})
export class ElearningBannerComponent implements OnInit {

    constructor() { }

    ngOnInit(): void {
    }

    @Input() featuredCourses: Course[] = [];

    bgImage = [
        {
            img: 'assets/img/gray-bg.jpg'
        }
    ]

    get displayCourses(): Course[] {
        return this.featuredCourses.slice(0, 2);
    }

}
