import { Component, OnInit } from '@angular/core';
import lgVideo from 'lightgallery/plugins/video';
import { BeforeSlideDetail } from 'lightgallery/lg-events';

@Component({
	selector: 'app-training-courses-details',
	templateUrl: './training-courses-details.component.html',
	styleUrls: ['./training-courses-details.component.scss']
})
export class TrainingCoursesDetailsComponent implements OnInit {

	constructor() { }

	ngOnInit(): void {}

    settings = {
        counter: false,
        plugins: [lgVideo]
    };

    // Tabs
    currentTab = 'tab1';
    switchTab(event: MouseEvent, tab: string) {
        event.preventDefault();
        this.currentTab = tab;
    }

}