import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule, ModalModule } from 'ngx-bootstrap';
import { RouterModule } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { NgxGalleryModule } from 'ngx-gallery';
import { FileUploadModule } from 'ng2-file-upload';
import { TimeAgoPipe } from 'time-ago-pipe';
import { NgxEchartsModule } from 'ngx-echarts';
import { NgxEditorModule } from 'ngx-editor';

import { AppRoutingModule } from './app-routing.module';
import { SharedModule } from './shared/shared.module';
import { InMemoryWebApiModule } from 'angular-in-memory-web-api';
import { InMemoryDataService } from './shared/inmemory-db/inmemory-db.service';


import { AppComponent } from './app.component';
import { NavPanelComponent } from './nav/nav-panel/nav-panel.component';
import { AuthService } from './_services/auth.service';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { AlertifyService } from './_services/alertify.service';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { appRoutes } from './routes';
import { AuthGuard } from './_guards/auth.guard';
import { UserService } from './_services/user.service';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail-resolver';
import { MemberListResolver } from './_resolvers/member-list-resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { PreventUnSavedChanges } from './_guards/prevent-unsaved-changes.guards';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { ListsResolver } from './_resolvers/lists.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver.';
import { MemberMessagesComponent } from './members/member-messages/member-messages.component';
// import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/hasRole.directive';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { PhotoManagementComponent } from './admin/photo-management/photo-management.component';
import { AdminService } from './_services/admin.service';
import { RolesModalComponent } from './admin/roles-modal/roles-modal.component';
import { LoginComponent } from './login/login.component';
import { ClassService } from './_services/class.service';
import { BooknoteComponent } from './admin/booknote/booknote.component';
import { HomePanelComponent } from './home/home-panel/home-panel.component';
import { UserCardComponent } from './users/user-card/user-card.component';
import { AgendaPanelComponent } from './agenda/agenda-panel/agenda-panel.component';
import { AgendaListComponent } from './agenda/agenda-list/agenda-list.component';
import { AgendaScheduleComponent } from './agenda/agenda-schedule/agenda-schedule.component';
import { NavStudentComponent } from './nav/nav-student/nav-student.component';
import { NavParentComponent } from './nav/nav-parent/nav-parent.component';
import { NavTeacherComponent } from './nav/nav-teacher/nav-teacher.component';
import { NavAdminComponent } from './nav/nav-admin/nav-admin.component';
import { ClassPanelComponent } from './classes/class-panel/class-panel.component';
import { StudentCardComponent } from './classes/student-card/student-card.component';
import { ClassNavComponent } from './classes/class-nav/class-nav.component';
import { AgendaItemComponent } from './classes/agenda-item/agenda-item.component';
import { ClassLifeFormComponent } from './classes/class-lifeForm/class-lifeForm.component';
import { NgZorroAntdModule, NZ_I18N, fr_FR } from 'ng-zorro-antd';
import { registerLocaleData, CommonModule } from '@angular/common';
import fr from '@angular/common/locales/fr';
import { ClassAbsenceComponent } from './classes/class-absence/class-absence.component';
import { ClassSanctionComponent } from './classes/class-sanction/class-sanction.component';
import { SchedulePanelComponent } from './schedule/schedule-panel/schedule-panel.component';
registerLocaleData(fr);
import { ScheduleDayComponent } from './schedule/schedule-day/schedule-day.component';
import { GradePanelComponent } from './grades/grade-panel/grade-panel.component';
import { SkillsModalComponent } from './grades/skills-modal/skills-modal.component';
import { SkillsComponent } from './grades/skills/skills.component';
import { EvaluationService } from './_services/evaluation.service';
import { ShowErrorsComponent } from './show-errors/show-errors.component';
import { GradeAddFormComponent } from './grades/grade-addForm/grade-addForm.component';
import { GradeListComponent } from './grades/grade-list/grade-list.component';
import { FirstStepFinishmentComponent } from './registration/firstStep-finishment/firstStep-finishment.component';
import { ConfirmEmailComponent } from './registration/confirm-email/confirm-email.component';
import { MemberPasswordSettingComponent } from './members/member-password-setting/member-password-setting.component';
import { PreInscriptionComponent } from './admin/pre-inscription/pre-inscription.component';
import { SigninComponent } from './views/sessions/signin/signin.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DataTablesRoutingModule } from './views/data-tables/data-tables-routing.module';
import { ListPaginationComponent } from './views/data-tables/list-pagination/list-pagination.component';
import { FullscreenTableComponent } from './views/data-tables/fullscreen-table/fullscreen-table.component';
import { PagingTableComponent } from './views/data-tables/paging-table/paging-table.component';
import { FilterTableComponent } from './views/data-tables/filter-table/filter-table.component';
import { AgendaModalComponent } from './agenda/agenda-modal/agenda-modal.component';
import { ClassStudentsComponent } from './classes/class-students/class-students.component';
import { ClassAgendaComponent } from './classes/class-agenda/class-agenda.component';
import { StudentDashboardComponent } from './views/dashboard/student-dashboard/student-dashboard.component';
import { TeacherDashboardComponent } from './views/dashboard/teacher-dashboard/teacher-dashboard.component';
import { UserHomeResolver } from './_resolvers/user-home-resolver';
import { AdminDashboardComponent } from './views/dashboard/admin-dashboard/admin-dashboard.component';
import { InscriptionComponent } from './views/forms/inscription/inscription.component';
import { WizardComponent } from './views/forms/wizard/wizard.component';
import { FormWizardModule } from './shared/components/form-wizard/form-wizard.module';
import { TextMaskModule } from 'angular2-text-mask';
import { EmailConfirmResolver } from './_resolvers/email-confirm.resolver';
import { ForgotComponent } from './views/sessions/forgot/forgot.component';
import { UiKitsModule } from './views/ui-kits/ui-kits.module';
// MDB Angular Pro
import { StepperModule, ToastModule, WavesModule, CarouselModule } from 'ng-uikit-pro-standard';
import { ResetPasswordComponent } from './registration/reset-password/reset-password.component';
import { ResetPasswordResolver } from './_resolvers/reset-password.resolver';
import { EvalAddFormComponent } from './grades/eval-addForm/eval-addForm.component';
import { ClassCallSheetComponent } from './classes/class-callSheet/class-callSheet.component';
import { FlipModule } from 'ngx-flip';
import { InscriptionsListComponent } from './admin/inscriptions-list/inscriptions-list.component';
import { AccountHistoryComponent } from './admin/account-history/account-history.component';
import { InscriptionPanelComponent } from './admin/inscription-panel/inscription-panel.component';
import { ClassesPanelComponent } from './admin/class-managemet/classes-panel/classes-panel.component';
import { NewClassComponent } from './admin/class-managemet/new-class/new-class.component';
import { CoursesPanelComponent } from './admin/courses-management/courses-panel/courses-panel.component';
import { NewCourseComponent } from './admin/courses-management/new-course/new-course.component';
// import { NewTeacherComponent } from './admin/teacher-management/new-teacher/new-teacher.component';
import { PreRegisterComponent } from './admin/selfs-registers/pre-register/pre-register.component';
import { SelfRegisterComponent } from './admin/selfs-registers/self-register/self-register.component';
import { ClassesListResolver } from './_resolvers/classes-list-resolver';
import { CoursesListResolver } from './_resolvers/courses-list.resolver';
import { TeacherManagementResolver } from './_resolvers/teacher-management.resolver';
import { CallSheetResolver } from './_resolvers/callSheet-resolver';
import { SharedPipesModule } from './shared/pipes/shared-pipes.module';
import { NavGPanelComponent } from './nav/navGPanel/navGPanel.component';
import { SideBarComponent } from './nav/SideBar/SideBar.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { GradeStudentComponent } from './grades/grade-student/grade-student.component';
import { NewUserComponent } from './admin/user-management/new-user/new-user.component';
import { ParentRegisterComponent } from './admin/selfs-registers/parent-register/parent-register.component';
import { TeacherAssignmentComponent } from './admin/teacher-management/teacher-assignment/teacher-assignment.component';
import localeFr from '@angular/common/locales/fr';
import { LevelClassesComponent } from './classes/level-classes/level-classes.component';
import { LevelClassesResolver } from './_resolvers/level-classes_resolver';
import { ClassLifeComponent } from './classes/class-life/class-life.component';
import { ColorPickerModule } from 'ngx-color-picker';
import { SchoolComponent } from './admin/school/school.component';
import { SchoolResolver } from './_resolvers/school-resolver';
import { ClassScheduleComponent } from './admin/class-schedule/class-schedule.component';
import { ClassResolver } from './_resolvers/class-resolver';
import { ClassdayScheduleComponent } from './admin/classday-schedule/classday-schedule.component';
import { AppFormsModule } from './views/forms/forms.module';
import { ModalScheduleComponent } from './admin/modal-schedule/modal-schedule.component';
import { SharedComponentsModule } from './shared/components/shared-components.module';
import { StudentResolver } from './_resolvers/student-resolver';
import { ProductsListComponent } from './admin/product/products-list/products-list.component';
import { ProductFormComponent } from './admin/product/product-form/product-form.component';
import { ClassLevelProductsComponent } from './admin/treso/class-level-products/class-level-products.component';
import { ClassLevelProdFormComponent } from './admin/treso/class-level-prod-form/class-level-prod-form.component';
import { DeadLineListComponent } from './admin/treso/dead-line-list/dead-line-list.component';
import { DeadLineFormComponent } from './admin/treso/dead-line-form/dead-line-form.component';
import { DeadLineListResolver } from './_resolvers/dead-line-list-resolver';
import { DeadLineFormResolver } from './_resolvers/dead-line-form-resolver';
import { ProductsListResolver } from './_resolvers/products-list-resolver';
import { ProductFormResolver } from './_resolvers/product-form-resolver';
import { CourseFormResolver } from './_resolvers/course-form-resolver';
import { TeacherFormResolver } from './_resolvers/teacher-form-resolver';

import { BroadcastComponent } from './comm/brodcast/broadcast.component';
import { MDBBootstrapModulesPro, MDBSpinningPreloader } from 'ng-uikit-pro-standard';
// import { MDBSpinningPreloader } from 'ng-uikit-pro-standard';
import { TagInputsComponent } from './views/forms/tag-inputs/tag-inputs.component';
import { TagInputModule } from 'ngx-chips';
import { EmailComponent } from './comm/email/email.component';
import { StudentAgendaComponent } from './agenda/student-agenda/student-agenda.component';
import { StudentLifeComponent } from './classes/student-life/student-life.component';
import { ClassTeachersComponent } from './classes/class-teachers/class-teachers.component';
import * as Chart from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { CourseCoefficientsComponent } from './admin/courses-management/course-coefficients/course-coefficients.component';
import { CoefficientFormComponent } from './admin/courses-management/coefficient-form/coefficient-form.component';
import { CoefficientFormResolver } from './_resolvers/coeffiient-form-form-resolver';
import { ChildrenListComponent } from './children-list/children-list.component';
import { UsersHeaderComponent } from './users-header/users-header.component';
import { PeriodicitiesComponent } from './admin/treso/periodicities/periodicities.component';
import { PeriodicityFormComponent } from './admin/treso/periodicity-form/periodicity-form.component';
import { PeriodicityFormResolver } from './_resolvers/periodicity-form-resolver';
import { PeriodicitiesListResolver } from './_resolvers/periodicities-list-resolver';
import { PayableAtsComponent } from './admin/treso/payableAts/payableAts.component';
import { PayableAtListResolver } from './_resolvers/payableAt-list-resolver';
import { PayableFormComponent } from './admin/treso/payable-form/payable-form.component';
import { PayableFormResolver } from './_resolvers/payable-form-resolver';
import { FullScreenDirective } from './_directives/full-screen.directive';
import { StudentScheduleComponent } from './schedule/student-schedule/student-schedule.component';
import { ConvertToPDFComponent } from './admin/_docs/convertToPDF/convertToPDF.component';
import { ImportFichierComponent } from './admin/import-fichier/import-fichier.component';
import { CallSheetCardComponent } from './classes/callSheet-card/callSheet-card.component';
import { SendSmsComponent } from './admin/sendSms/sendSms.component';
import { UserAccountComponent } from './users/user-account/user-account.component';
import { UserAccountResolver } from './_resolvers/user-account-resolver';
import { SmsTemplateComponent } from './admin/sms-template/sms-template.component';
import { AddSmsTemplateComponent } from './admin/add-smsTemplate/add-smsTemplate.component';
import { SmsTemplateResolver } from './_resolvers/sms-template-resolver';
import { EditSmsTemplateResolver } from './_resolvers/edit-sms-template-resolver';
import { AddUserGradesComponent } from './grades/add-user-grades/add-user-grades.component';
import { ClassGradesResolver } from './_resolvers/class-grades-resolver';
import { ChildSmsCardComponent } from './users/child-sms-card/child-sms-card.component';
import { NavNotLoggedComponent } from './nav/navNotLogged/navNotLogged.component';
import { RegisterChildCardComponent } from './admin/selfs-registers/register-child-card/register-child-card.component';
import { ClassSessionComponent } from './classes/class-session/class-session.component';
import { ClassProgressComponent } from './classes/class-progress/class-progress.component';
import { TeacherProgramComponent } from './classes/teacher-program/teacher-program.component';
import { NewThemeComponent } from './programs/new-theme/new-theme.component';
import { ThemesListComponent } from './programs/new-theme/themes-list/themes-list.component';

import { TeacherProgramResolver } from './_resolvers/teacher-program-resolver';
import { ClassProgramDataComponent } from './classes/class-program-data/class-program-data.component';
import { TimelineComponent } from './shared/components/timeline/timeline.component';
import { TeacherManagementComponent } from './admin/teacher-management/teacher-management.component';
import { NewTeacherComponent } from './admin/teacher-management/new-teacher/new-teacher.component';
import { ClassSessionResolver } from './_resolvers/classSession-resolver';
import { AddClassLifeComponent } from './classes/add-classLife/add-classLife.component';
import { ImportUsersComponent } from './admin/selfs-registers/import-users/import-users.component';
import { EmailTemplateComponent } from './admin/email-template/email-template.component';
import { EmailTemplateResolver } from './_resolvers/email-template-resolver';
import { AddEmailTemplateComponent } from './admin/add-emailTemplate/add-emailTemplate.component';
import { EditEmailTemplateResolver } from './_resolvers/edit-email-template-resolver';
import { EditTeacherResolver } from './_resolvers/edit-teacher-resolver';
import { CourseShowingComponent } from './classes/course-showing/course-showing.component';
import { ClassCardComponent } from './admin/class-managemet/class-card/class-card.component';
import { BtnBackDirective } from './_directives/btnBack.directive';

// the second parameter 'fr' is optional
registerLocaleData(localeFr, 'fr');

Chart.plugins.unregister(ChartDataLabels);

export function tokenGetter() {
   return localStorage.getItem('token');
}

@NgModule({
   declarations: [
      AppComponent,
      NavPanelComponent,
      HomePanelComponent,
      MemberListComponent,
      ListsComponent,
      MessagesComponent,
      MemberCardComponent,
      MemberDetailComponent,
      MemberEditComponent,
      PhotoEditorComponent,
      TimeAgoPipe,
      MemberMessagesComponent,
      // AdminPanelComponent,
      HasRoleDirective,
      FullScreenDirective,
      BtnBackDirective,
      UserManagementComponent,
      PhotoManagementComponent,
      RolesModalComponent,
      LoginComponent,
      BooknoteComponent,
      UserCardComponent,
      AgendaPanelComponent,
      AgendaListComponent,
      AgendaScheduleComponent,
      NavStudentComponent,
      NavParentComponent,
      NavTeacherComponent,
      NavAdminComponent,
      ClassPanelComponent,
      StudentCardComponent,
      ClassNavComponent,
      AgendaItemComponent,
      ClassLifeFormComponent,
      ClassAbsenceComponent,
      ClassSanctionComponent,
      SchedulePanelComponent,
      ScheduleDayComponent,
      GradePanelComponent,
      SkillsModalComponent,
      SkillsComponent,
      GradeAddFormComponent,
      GradeListComponent,
      ShowErrorsComponent,
      FirstStepFinishmentComponent,
      ConfirmEmailComponent,
      PreInscriptionComponent,
      MemberPasswordSettingComponent,
      TeacherManagementComponent,
      // CoursClassesManagementComponent,
      // UserTypesComponent,
      // AdministrationUsersComponent,
      // UserListComponent,
      SigninComponent,
      FullscreenTableComponent,
      PagingTableComponent,
      FilterTableComponent,
      ListPaginationComponent,
      AgendaModalComponent,
      ClassStudentsComponent,
      ClassAgendaComponent,
      StudentDashboardComponent,
      TeacherDashboardComponent,
      AdminDashboardComponent,
      InscriptionComponent,
      WizardComponent,
      ForgotComponent,
      ResetPasswordComponent,
      EvalAddFormComponent,
      ClassCallSheetComponent,
      InscriptionsListComponent,
      InscriptionPanelComponent,
      AccountHistoryComponent,
      ClassesPanelComponent,
      NewClassComponent,
      CoursesPanelComponent,
      NewCourseComponent,
      NewTeacherComponent,
      PreRegisterComponent,
      SelfRegisterComponent,
      NavGPanelComponent,
      SideBarComponent,
      GradeStudentComponent,
      NewUserComponent,
      ParentRegisterComponent,
      TeacherAssignmentComponent,
      AccountHistoryComponent,
      LevelClassesComponent,
      ClassLifeComponent,
      SchoolComponent,
      ClassScheduleComponent,
      ClassdayScheduleComponent,
      ModalScheduleComponent,
      ProductsListComponent,
      ProductFormComponent,
      ClassLevelProductsComponent,
      ClassLevelProdFormComponent,
      DeadLineListComponent,
      DeadLineFormComponent,
      BroadcastComponent,
      TagInputsComponent,
      EmailComponent,
      StudentAgendaComponent,
      StudentLifeComponent,
      ClassTeachersComponent,
      CourseCoefficientsComponent,
      CoefficientFormComponent,
      ChildrenListComponent,
      UsersHeaderComponent,
      PeriodicitiesComponent,
      PeriodicityFormComponent,
      PayableAtsComponent,
      PayableFormComponent,
      StudentScheduleComponent,
      ConvertToPDFComponent,
      ImportFichierComponent,
      CallSheetCardComponent,
      SendSmsComponent,
      UserAccountComponent,
      SmsTemplateComponent,
      AddSmsTemplateComponent,
      AddUserGradesComponent,
      ChildSmsCardComponent,
      NavNotLoggedComponent,
      RegisterChildCardComponent,
      ClassSessionComponent,
      ClassProgressComponent,
      TeacherProgramComponent,
      NewThemeComponent,
      ThemesListComponent,
      ClassProgramDataComponent,
      TimelineComponent,
      AddClassLifeComponent,
      ImportUsersComponent,
      EmailTemplateComponent,
      AddEmailTemplateComponent,
      CourseShowingComponent,
      ClassCardComponent
   ],
   imports: [
      FormWizardModule,
      SharedModule,
      HttpClientModule,
      InMemoryWebApiModule.forRoot(InMemoryDataService, { passThruUnknownUrl: true }),
      AppRoutingModule,
      BrowserModule,
      BrowserAnimationsModule,
      NgZorroAntdModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      BsDropdownModule.forRoot(),
      BsDatepickerModule.forRoot(),
      PaginationModule.forRoot(),
      TabsModule.forRoot(),
      ButtonsModule.forRoot(),
      RouterModule.forRoot(appRoutes),
      // ModalModule.forRoot(),
      ModalModule,
      NgxGalleryModule,
      FlipModule,
      FileUploadModule,
      JwtModule.forRoot({
         config: {
            tokenGetter: tokenGetter,
            whitelistedDomains: ['localhost:5000'],
            blacklistedRoutes: ['localhost:5000/api/auth']
         }
      }),
      CommonModule,
      NgxPaginationModule,
      NgxDatatableModule,
      NgbModule,
      DataTablesRoutingModule,
      TextMaskModule,
      UiKitsModule,
      StepperModule,
      WavesModule,
      CarouselModule,
      ToastModule.forRoot(),
      SharedPipesModule,
      PerfectScrollbarModule,
      NgxEchartsModule,
      ColorPickerModule,
      SharedComponentsModule,
      AppFormsModule,
      MDBBootstrapModulesPro.forRoot(),
      TagInputModule,
      NgxEditorModule
   ],
   providers: [
      AuthService,
      ErrorInterceptorProvider,
      AlertifyService,
      AuthGuard,
      UserService,
      MemberDetailResolver,
      MemberListResolver,
      MemberEditResolver,
      PreventUnSavedChanges,
      ListsResolver,
      MessagesResolver,
      EmailConfirmResolver,
      ResetPasswordResolver,
      AdminService,
      ClassService,
      EvaluationService,
      UserHomeResolver,
      EmailConfirmResolver,
      ResetPasswordResolver,
      ClassesListResolver,
      CoursesListResolver,
      TeacherManagementResolver,
      LevelClassesResolver,
      CallSheetResolver,
      SchoolResolver,
      ClassResolver,
      StudentResolver,
      MDBSpinningPreloader,
      DeadLineListResolver,
      DeadLineFormResolver,
      ProductsListResolver,
      ProductFormResolver,
      CourseFormResolver,
      TeacherFormResolver,
      CoefficientFormResolver,
      PeriodicitiesListResolver,
      PeriodicityFormResolver,
      PayableAtListResolver,
      PayableFormResolver,
      UserAccountResolver,
      SmsTemplateResolver,
      EditSmsTemplateResolver,
      ClassGradesResolver,
      TeacherProgramResolver,
      ClassSessionResolver,
      EmailTemplateResolver,
      EditEmailTemplateResolver,
      EditTeacherResolver,
      { provide: NZ_I18N, useValue: fr_FR }
      // { provide: NZ_ICON_DEFAULT_TWOTONE_COLOR, useValue: '#00ff00' },
      // { provide: NZ_ICONS, useValue: icons }
   ],
   entryComponents: [
      RolesModalComponent,
      SkillsModalComponent,
      AgendaModalComponent,
      ModalScheduleComponent
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
