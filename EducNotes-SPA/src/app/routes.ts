import {Routes} from '@angular/router';

import { HomePanelComponent } from './home/home-panel/home-panel.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail-resolver';
import { MemberListResolver } from './_resolvers/member-list-resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { PreventUnSavedChanges } from './_guards/prevent-unsaved-changes.guards';
import { ListsResolver } from './_resolvers/lists.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver.';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { AgendaPanelComponent } from './agenda/agenda-panel/agenda-panel.component';
import { ClassPanelComponent } from './classes/class-panel/class-panel.component';
import { GradePanelComponent } from './grades/grade-panel/grade-panel.component';
import { ClassStudentsComponent } from './classes/class-students/class-students.component';
import { ClassAgendaComponent } from './classes/class-agenda/class-agenda.component';
import { StudentDashboardComponent } from './views/dashboard/student-dashboard/student-dashboard.component';
import { UserHomeResolver } from './_resolvers/user-home-resolver';
import { ParentDashboardComponent } from './views/dashboard/parent-dashboard/parent-dashboard.component';
import { TeacherDashboardComponent } from './views/dashboard/teacher-dashboard/teacher-dashboard.component';

import { InscriptionComponent } from './views/forms/inscription/inscription.component';
import { ConfirmEmailComponent } from './registration/confirm-email/confirm-email.component';
import { EmailConfirmResolver } from './_resolvers/email-confirm.resolver';
import { ForgotComponent } from './views/sessions/forgot/forgot.component';
import { ResetPasswordResolver } from './_resolvers/reset-password.resolver';
import { ResetPasswordComponent } from './registration/reset-password/reset-password.component';
import { EvalAddFormComponent } from './grades/eval-addForm/eval-addForm.component';
import { ClassCallSheetComponent } from './classes/class-callSheet/class-callSheet.component';
import { AdminDashboardComponent } from './views/dashboard/admin-dashboard/admin-dashboard.component';
import { InscriptionsListComponent } from './admin/inscriptions-list/inscriptions-list.component';
import { ClassesPanelComponent } from './admin/class-managemet/classes-panel/classes-panel.component';
import { ClassesListResolver } from './_resolvers/classes-list-resolver';
import { CoursesPanelComponent } from './admin/courses-management/courses-panel/courses-panel.component';
import { CoursesListResolver } from './_resolvers/courses-list.resolver';
import { TeacherManagementComponent } from './admin/teacher-management/teacher-management.component';
import { PreRegisterComponent } from './admin/selfs-registers/pre-register/pre-register.component';
import { TeacherManagementResolver } from './_resolvers/teacher-management.resolver';
import { SelfRegisterComponent } from './admin/selfs-registers/self-register/self-register.component';
import { CallSheetResolver } from './_resolvers/callSheet-resolver';
import { GradeStudentComponent } from './grades/grade-student/grade-student.component';
import { SigninComponent } from './views/sessions/signin/signin.component';
import { LevelClassesComponent } from './classes/level-classes/level-classes.component';
import { LevelClassesResolver } from './_resolvers/level-classes_resolver';
import { ClassLifeComponent } from './classes/class-life/class-life.component';
import { SchoolComponent } from './admin/school/school.component';
import { SchoolResolver } from './_resolvers/school-resolver';
import { ClassScheduleComponent } from './admin/class-schedule/class-schedule.component';
import { SchedulePanelComponent } from './schedule/schedule-panel/schedule-panel.component';
import { ClassResolver } from './_resolvers/class-resolver';
import { StudentResolver } from './_resolvers/student-resolver';
import { BroadcastComponent } from './comm/brodcast/broadcast.component';
import { EmailComponent } from './comm/email/email.component';
import { Component } from '@angular/core';
import { NewTeacherComponent } from './admin/teacher-management/new-teacher/new-teacher.component';

export const appRoutes: Routes = [
    {path: '', component: SigninComponent},
    {path: 'forgotPassword', component: ForgotComponent},
    {path: 'confirmEmail/:code', component: ConfirmEmailComponent, resolve : {user: EmailConfirmResolver}},
    {path: 'resetPassword/:code', component: ResetPasswordComponent, resolve : {user: ResetPasswordResolver}},
    {path: 'selfRegister/:code', component: SelfRegisterComponent, resolve : {user: EmailConfirmResolver}},

    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            {path: 'home', component: HomePanelComponent, resolve: {user: UserHomeResolver}},
            {path: 'broadcast', component: BroadcastComponent},
            {path: 'sendEmail', component: EmailComponent},
            {path: 'members', component: MemberListComponent, resolve: {users : MemberListResolver}},
            {path: 'members/:id', component: MemberDetailComponent, resolve: {user : MemberDetailResolver}},
            {path: 'member/edit', component: MemberEditComponent,
              resolve: {user: MemberEditResolver}, canDeactivate: [PreventUnSavedChanges]},
            {path: 'messages', component: MessagesComponent, resolve: {messages: MessagesResolver}},
            {path: 'lists', component: ListsComponent, resolve: {users: ListsResolver}},
            {path: 'admin', component: AdminPanelComponent, data: {roles: ['Admin', 'Moderator']}},
            {path: 'agenda', component: AgendaPanelComponent},
            {path: 'classes', component: ClassPanelComponent},
            {path: 'notes', component: GradePanelComponent},
            {path: 'classLife/:classId', component: ClassLifeComponent},
            {path: 'studentNotes/:id', component: GradeStudentComponent},
            {path: 'classScheduleEdit', component: ClassScheduleComponent},
            {path: 'studentsClass/:classId', component: ClassStudentsComponent},
            {path: 'agendas/:classId', component: ClassAgendaComponent},
            {path: 'student', component: StudentDashboardComponent},
            {path: 'studentFromP/:id', component: StudentDashboardComponent},
            {path: 'parent', component: ParentDashboardComponent, resolve: {parent: UserHomeResolver}},
            {path: 'teacher', component: TeacherDashboardComponent, resolve: {teacher: UserHomeResolver}},
            {path: 'admins', component: AdminDashboardComponent, resolve: {admin: UserHomeResolver}},
            {path: 'inscriptions', component: InscriptionComponent},
            {path: 'agendas/:classId', component: ClassAgendaComponent},
            {path: 'addEval', component: EvalAddFormComponent},
            {path: 'callSheet/:id', component: ClassCallSheetComponent, resolve: {schedule: CallSheetResolver}},
            {path: 'inscriptions', component: InscriptionComponent},
            {path: 'inscriptionsList', component: InscriptionsListComponent},
            {path: 'classesPanel', component: ClassesPanelComponent, resolve: {levels: ClassesListResolver}},
            {path: 'classSchedule/:classId', component: SchedulePanelComponent, resolve: {class: ClassResolver}},
            {path: 'courses', component: CoursesPanelComponent, resolve: {courses: CoursesListResolver}},
            {path: 'teachers', component: TeacherManagementComponent, resolve: {teachers: TeacherManagementResolver}},
            {path: 'editTeacher/:id', component: NewTeacherComponent, resolve: {teacher: TeacherManagementResolver}},
            {path: 'preregister', component: PreRegisterComponent},
            {path: 'levelClasses/:levelId', component: LevelClassesComponent, resolve : {classes: LevelClassesResolver}},
            {path: 'studentGrades/:id', component: GradeStudentComponent, resolve: {student: StudentResolver}},
            {path: 'school', component: SchoolComponent, resolve : {school: SchoolResolver}}
        ]
    },
    {path: '**', redirectTo: '', pathMatch: 'full'}
];
