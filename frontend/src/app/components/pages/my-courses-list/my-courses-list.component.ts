import { Component, OnInit } from '@angular/core';
import { StudentServices } from 'src/app/core/services/student.service';

@Component({
  selector: 'app-my-courses-list',
  templateUrl: './my-courses-list.component.html',
  styleUrls: ['./my-courses-list.component.css']
})
export class MyCoursesListComponent implements OnInit {
  courses: any[] = [];
  isLoading = true;
  activeTab = 'all';

  constructor(private studentServices: StudentServices) {}

  ngOnInit() {
    this.loadCourses();
  }

  loadCourses() {
    this.isLoading = true;
    this.studentServices.getListStudentProcess().subscribe((res: any) => {
      if (res.retCode === 0) {
        this.courses = res.data || [];
      }
      this.isLoading = false;
    }, () => { this.isLoading = false; });
  }

  get filteredCourses() {
    if (this.activeTab === 'completed') return this.courses.filter(c => (c.processPercent || 0) >= 100);
    if (this.activeTab === 'inprogress') return this.courses.filter(c => (c.processPercent || 0) > 0 && (c.processPercent || 0) < 100);
    return this.courses;
  }
}
