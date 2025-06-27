import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { AdminDashboardComponent } from './admin/admin-dashboard/admin-dashboard.component';
import { UserDashboardComponent } from './user/user-dashboard/user-dashboard.component';
import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';
import { ReviewComponent} from './user/review/review.component';
import { TakeExamComponent } from './user/take-exam/take-exam.component';
import { ManageQuestionsComponent } from './admin/manage-questions/manage-questions.component';
import { ManageExamsComponent } from './admin/manage-exams/manage-exams.component';

const routes: Routes = [
  {
    path: 'admin',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
    children: [
      { path: 'dashboard', component: AdminDashboardComponent },
      { path: 'questions', component: ManageQuestionsComponent },
      { path: 'exams', component: ManageExamsComponent },

    ]
  },
  {
    path: 'user',
    canActivate: [AuthGuard],
    children: [
      { path: 'dashboard', component: UserDashboardComponent },
      { path: 'take-exam/:id', component: TakeExamComponent },
      { path: 'review/:id', component: ReviewComponent} ,
    ]
  },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
