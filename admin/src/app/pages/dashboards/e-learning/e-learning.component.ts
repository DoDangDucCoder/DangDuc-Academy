import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

interface AdminDashboardModel {
  totalStudents: number;
  totalTeachers: number;
  totalCourses: number;
  totalRevenue: number;
  recentOrders: any[];
}

@Component({
  selector: 'app-e-learning',
  templateUrl: './e-learning.component.html',
  styleUrls: ['./e-learning.component.scss']
})
export class ELearningComponent implements OnInit {

  dashboardData: AdminDashboardModel = {
    totalStudents: 0,
    totalTeachers: 0,
    totalCourses: 0,
    totalRevenue: 0,
    recentOrders: []
  };

  loading: boolean = true;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchDashboardData();
  }

  fetchDashboardData(): void {
    this.loading = true;
    this.http.get<any>(`${environment.baseUrl}admin-dashboard`).subscribe({
      next: (res) => {
        if (res.data) {
          this.dashboardData = res.data;
        }
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching dashboard data:', err);
        this.loading = false;
      }
    });
  }

}
