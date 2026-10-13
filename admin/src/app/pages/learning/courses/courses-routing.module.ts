import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// Component
import { ListCourseComponent } from './list-courses/list-courses.component';
import { CategoryComponent } from '../../manager-system/category/category.component';
import { OverviewComponent } from './overview/overview.component';
import { CreateCourseComponent } from './create-course/create-course.component';
import { CreateCourseDetailsComponent } from './create-course-details/create-course-details.component';
import { EditCourseComponent } from './edit-course/edit-course.component';
import { InstructorDashboardComponent } from './instructor-dashboard/instructor-dashboard.component';
import { QuizManagerComponent } from './quiz-manager/quiz-manager.component';
import { LiveClassManagerComponent } from './live-class-manager/live-class-manager.component';
import { AssignmentManagerComponent } from './assignment-manager/assignment-manager.component';

const routes: Routes = [
  {
    path: "list-course",
    component: ListCourseComponent
  },

  {
    path: "course-overview/:idCourse",
    component: OverviewComponent
  },
  {
    path: "create-course",
    component: CreateCourseComponent
  },
  {
    path: "create-course-details/:idCourse",
    component: CreateCourseDetailsComponent
  },
  {
    path: "edit-course/:id",
    component: EditCourseComponent
  },
  {
    path: "instructor-dashboard",
    component: InstructorDashboardComponent
  },
  {
    path: "quiz-manager",
    component: QuizManagerComponent
  },
  {
    path: "live-class-manager",
    component: LiveClassManagerComponent
  },
  {
    path: "assignment-manager",
    component: AssignmentManagerComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CoursesRoutingModule { }
