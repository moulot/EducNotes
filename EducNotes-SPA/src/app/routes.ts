import { Routes } from '@angular/router';
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
import { AgendaPanelComponent } from './agenda/agenda-panel/agenda-panel.component';
import { ClassPanelComponent } from './classes/class-panel/class-panel.component';
import { GradePanelComponent } from './grades/grade-panel/grade-panel.component';
import { ClassStudentsComponent } from './classes/class-students/class-students.component';
import { ClassAgendaComponent } from './classes/class-agenda/class-agenda.component';
import { UserHomeResolver } from './_resolvers/user-home-resolver';
import { TeacherDashboardComponent } from './dashboard/teacher-dashboard/teacher-dashboard.component';
import { ConfirmEmailComponent } from './registration/confirm-email/confirm-email.component';
import { ForgotComponent } from './views/sessions/forgot/forgot.component';
import { ResetPasswordComponent } from './registration/reset-password/reset-password.component';
import { EvalAddFormComponent } from './grades/eval-addForm/eval-addForm.component';
import { ClassCallSheetComponent } from './classes/class-callSheet/class-callSheet.component';
import { AdminDashboardComponent } from './dashboard/admin-dashboard/admin-dashboard.component';
import { InscriptionsListComponent } from './admin/inscriptions-list/inscriptions-list.component';
import { ClassesPanelComponent } from './admin/class-management/classes-panel/classes-panel.component';
import { ClassesListResolver } from './_resolvers/classes-list-resolver';
import { CoursesPanelComponent } from './admin/courses-management/courses-panel/courses-panel.component';
import { CoursesListResolver } from './_resolvers/courses-list.resolver';
import { TeacherManagementResolver } from './_resolvers/teacher-management.resolver';
import { CallSheetResolver } from './_resolvers/callSheet-resolver';
import { GradeStudentComponent } from './grades/grade-student/grade-student.component';
import { SigninComponent } from './views/sessions/signin/signin.component';
import { LevelClassesComponent } from './classes/level-classes/level-classes.component';
import { LevelClassesResolver } from './_resolvers/level-classes_resolver';
import { ClassLifeComponent } from './classes/class-life/class-life.component';
import { ClassScheduleComponent } from './admin/class-schedule/class-schedule.component';
import { SchedulePanelComponent } from './schedule/schedule-panel/schedule-panel.component';
import { ClassResolver } from './_resolvers/class-resolver';
import { AppImgCropperComponent } from './views/forms/img-cropper/img-cropper.component';
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
import { NewCourseComponent } from './admin/courses-management/new-course/new-course.component';
import { CourseFormResolver } from './_resolvers/course-form-resolver';
import { NewTeacherComponent } from './admin/teacher-management/new-teacher/new-teacher.component';
import { TeacherFormResolver } from './_resolvers/teacher-form-resolver';
import { TeacherAssignmentComponent } from './admin/teacher-management/teacher-assignment/teacher-assignment.component';
import { NewClassComponent } from './admin/class-management/new-class/new-class.component';
import { BroadcastComponent } from './comm/brodcast/broadcast.component';
import { EmailComponent } from './comm/email/email.component';
import { StudentAgendaComponent } from './agenda/student-agenda/student-agenda.component';
import { StudentLifeComponent } from './classes/student-life/student-life.component';
import { ClassTeachersComponent } from './classes/class-teachers/class-teachers.component';
import { CourseCoefficientsComponent } from './admin/courses-management/course-coefficients/course-coefficients.component';
import { CoefficientFormComponent } from './admin/courses-management/coefficient-form/coefficient-form.component';
import { CoefficientFormResolver } from './_resolvers/coeffiient-form-form-resolver';
import { PeriodicitiesComponent } from './admin/treso/periodicities/periodicities.component';
import { PeriodicityFormComponent } from './admin/treso/periodicity-form/periodicity-form.component';
import { PeriodicityFormResolver } from './_resolvers/periodicity-form-resolver';
import { PeriodicitiesListResolver } from './_resolvers/periodicities-list-resolver';
import { PayableAtListResolver } from './_resolvers/payableAt-list-resolver';
import { PayableAtsComponent } from './admin/treso/payableAts/payableAts.component';
import { PayableFormComponent } from './admin/treso/payable-form/payable-form.component';
import { PayableFormResolver } from './_resolvers/payable-form-resolver';
import { StudentDashboardComponent } from './dashboard/student-dashboard/student-dashboard.component';
import { StudentScheduleComponent } from './schedule/student-schedule/student-schedule.component';
import { ConvertToPDFComponent } from './admin/_docs/convertToPDF/convertToPDF.component';
import { ImportFichierComponent } from './admin/import-fichier/import-fichier.component';
import { SendSmsComponent } from './admin/sendSms/sendSms.component';
import { UserAccountComponent } from './users/user-account/user-account.component';
import { SmsTemplateComponent } from './admin/sms-template/sms-template.component';
import { AddSmsTemplateComponent } from './admin/add-smsTemplate/add-smsTemplate.component';
import { SmsTemplateResolver } from './_resolvers/sms-template-resolver';
import { EditSmsTemplateResolver } from './_resolvers/edit-sms-template-resolver';
import { AddUserGradesComponent } from './grades/add-user-grades/add-user-grades.component';
import { ClassGradesResolver } from './_resolvers/class-grades-resolver';
import { ClassProgressComponent } from './classes/class-progress/class-progress.component';
import { ClassSessionComponent } from './classes/class-session/class-session.component';
import { TeacherProgramComponent } from './classes/teacher-program/teacher-program.component';
import { ThemesListComponent } from './programs/new-theme/themes-list/themes-list.component';
import { NewThemeComponent } from './programs/new-theme/new-theme.component';
import { TeacherProgramResolver } from './_resolvers/teacher-program-resolver';
import { TeacherManagementComponent } from './admin/teacher-management/teacher-management.component';
import { ClassSessionResolver } from './_resolvers/classSession-resolver';
import { AddClassLifeComponent } from './classes/add-classLife/add-classLife.component';
import { EmailTemplateResolver } from './_resolvers/email-template-resolver';
import { EmailTemplateComponent } from './admin/email-template/email-template.component';
import { AddEmailTemplateComponent } from './admin/add-emailTemplate/add-emailTemplate.component';
import { EditEmailTemplateResolver } from './_resolvers/edit-email-template-resolver';
import { EditTeacherResolver } from './_resolvers/edit-teacher-resolver';
import { CourseShowingComponent } from './classes/course-showing/course-showing.component';
import { ClassStudentsAssignmentComponent } from './classes/class-students-assignment/class-students-assignment.component';
import { TuitionPanelComponent } from './admin/tuition/tuition-panel/tuition-panel.component';
import { CheckoutComponent } from './tuition/checkout/checkout.component';
import { CheckoutResolver } from './_resolvers/checkout-resolver';
import { NewTuitionComponent } from './admin/tuition/new-tuition/new-tuition.component';
import { ContactUsComponent } from './contactus/contact-us/contact-us.component';
import { EditChildrenComponent } from './users/edit-children/edit-children.component';
import { EditChildrenResolver } from './_resolvers/edit-children-resolver';
import { InvalidAccountComponent } from './registration/invalid-account/invalid-account.component';
import { TreasuryComponent } from './admin/treso/treasury/treasury.component';
import { RolesComponent } from './admin/roles/roles.component';
import { ChildFileResolver } from './_resolvers/child-file-resolver';
import { AddPaymentComponent } from './admin/add-payment/add-payment.component';
import { AddPaymentResolver } from './_resolvers/add-payment-resolver';
import { TuitionListComponent } from './admin/tuition/tuition-list/tuition-list.component';
import { TuitionListResolver } from './_resolvers/tuition-list-resolver';
import { TuitionDetailsComponent } from './admin/tuition/tuition-details/tuition-details.component';
import { TuitionDetailsResolver } from './_resolvers/tuition-details-resolver';
import { ValidatePaymentsComponent } from './admin/validate-payments/validate-payments.component';
import { ValidatePaymentsResolver } from './_resolvers/validate-payments-resolver';
import { TuitionFeesComponent } from './admin/tuition/tuition-fees/tuition-fees.component';
import { TuitionFeesResolver } from './_resolvers/tuition-fees-resolver';
import { RecoveryListComponent } from './admin/recovery-list/recovery-list.component';
import { PaymentLevelChildComponent } from './admin/payment-level-child/payment-level-child.component';
import { PaymentLevelChildResolver } from './_resolvers/payment-level-child-resolver';
import { AbsencesComponent } from './admin/classLife/absences/absences.component';
import { ChildFileComponent } from './admin/userFiles/child-file/child-file.component';
import { ParentFileComponent } from './admin/userFiles/parent-file/parent-file.component';
import { TeacherFileComponent } from './admin/userFiles/teacher-file/teacher-file.component';
import { ParentFileResolver } from './_resolvers/parent-file-resolver';
import { UserValidationComponent } from './admin/user-validation/user-validation.component';
import { UserValidationResolver } from './_resolvers/user-validation-resolver';
import { LockoutComponent } from './views/sessions/lockout/lockout.component';
import { EditAccountComponent } from './users/edit-account/edit-account.component';
import { EditAccountResolver } from './_resolvers/edit-account-resolver';
import { ClassScheduleEditComponent } from './admin/class-schedule-edit/class-schedule-edit.component';
import { ConfigComponent } from './admin/config/config/config.component';
import { ClassScheduleResolver } from './_resolvers/class-schedule-resolver';
import { EditLevelClassesComponent } from './admin/class-management/edit-level-classes/edit-level-classes.component';
import { RecoveryComponent } from './admin/treso/recovery/recovery.component';
import { RecoveryResolver } from './_resolvers/recovery-resolver';
import { SchoolSettingsComponent } from './admin/school-settings/school-settings.component';
import { SchoolSettingsResolver } from './_resolvers/school-settings-resolver';
import { TuitionDataComponent } from './admin/tuition/tuition-data/tuition-data.component';
import { RolesResolver } from './_resolvers/roles-resolver';
import { EmployeesComponent } from './admin/users/employees/employees.component';
import { EmployeesResolver } from './_resolvers/employees-resolver';
import { AddEmployeeComponent } from './admin/users/add-employee/add-employee.component';
import { AddRoleComponent } from './admin/add-role/add-role.component';
import { ConfirmUserEmailComponent } from './registration/confirm-user-email/confirm-user-email.component';
import { EditRoleResolver } from './_resolvers/edit-role-resolver';
import { AddServiceResolver } from './_resolvers/add-service-resolver';
import { ServicesComponent } from './admin/services/services/services.component';
import { ServicesResolver } from './_resolvers/services-resolver';
import { AddServiceComponent } from './admin/services/add-service/add-service.component';
import { DuedateDetailsComponent } from './admin/treso/duedate-details/duedate-details.component';
import { DuedateDetailsResolver } from './_resolvers/duedate-details-resolver';
import { ChildrenRecoveryComponent } from './admin/treso/children-recovery/children-recovery.component';
import { ChildLatePaymentsResolver } from './_resolvers/child-latePayments-resolver';
import { LevelRecoveryComponent } from './admin/treso/level-recovery/level-recovery.component';

export const appRoutes: Routes = [
    { path: 'forgotPassword', component: ForgotComponent },
    { path: 'imgCropper', component: AppImgCropperComponent },
    { path: 'signIn', component: SigninComponent },
    { path: 'confirmEmail', component: ConfirmEmailComponent},
    { path: 'confirmUserEmail', component: ConfirmUserEmailComponent},
    { path: 'contactus', component: ContactUsComponent},
    { path: 'resetPassword', component: ResetPasswordComponent},
    { path: 'lockout', component: LockoutComponent},

    {
      path: '',
      runGuardsAndResolvers: 'always',
      canActivate: [AuthGuard],
      children: [
        { path: '', component: HomePanelComponent },
        { path: 'home', component: HomePanelComponent, resolve: { user: UserHomeResolver } },
        { path: 'broadcast', component: BroadcastComponent },
        { path: 'fichier', component: ImportFichierComponent },
        { path: 'sendEmail', component: EmailComponent },
        { path: 'members', component: MemberListComponent, resolve: { users: MemberListResolver } },
        { path: 'members/:id', component: MemberDetailComponent, resolve: { user: MemberDetailResolver } },
        { path: 'member/edit', component: MemberEditComponent,
          resolve: { user: MemberEditResolver }, canDeactivate: [PreventUnSavedChanges]
        },
        { path: 'messages', component: MessagesComponent, resolve: { messages: MessagesResolver } },
        { path: 'lists', component: ListsComponent, resolve: { users: ListsResolver } },
        { path: 'agenda/:classId', component: AgendaPanelComponent },
        { path: 'classes', component: ClassPanelComponent },
        { path: 'grades', component: GradePanelComponent },
        { path: 'classLife/:classId', component: ClassLifeComponent },
        { path: 'studentLife/:id', component: StudentLifeComponent },
        { path: 'studentLifeP/:id', component: StudentLifeComponent },
        { path: 'studentNotes/:id', component: GradeStudentComponent },
        { path: 'classSchedules', component: ClassScheduleComponent, resolve: {classes: ClassScheduleResolver} },
        { path: 'classScheduleEdit/:classId', component: ClassScheduleEditComponent },
        { path: 'studentsClass/:classId', component: ClassStudentsComponent },
        { path: 'agendas/:classId', component: ClassAgendaComponent },
        { path: 'student', component: StudentDashboardComponent },
        { path: 'studentFromP/:id', component: StudentDashboardComponent },
        { path: 'teacher', component: TeacherDashboardComponent, resolve: { teacher: UserHomeResolver } },
        { path: 'admins', component: AdminDashboardComponent, resolve: { admin: UserHomeResolver } },
        { path: 'classStaff/:id', component: ClassTeachersComponent },
        { path: 'classStaffP/:id', component: ClassTeachersComponent },
        { path: 'studentSchedule/:id', component: StudentScheduleComponent },
        { path: 'studentScheduleP/:id', component: StudentScheduleComponent },
        { path: 'addEval', component: EvalAddFormComponent },
        { path: 'callSheet/:id', component: ClassCallSheetComponent, resolve: { session: CallSheetResolver } },
        { path: 'classSession/:id', component: ClassSessionComponent, resolve: { session: ClassSessionResolver } },
        { path: 'studentsToClass', component: InscriptionsListComponent },
        { path: 'classesPanel', component: ClassesPanelComponent, resolve: { levels: ClassesListResolver } },
        { path: 'addClass', component: NewClassComponent },
        { path: 'classSchedule/:classId', component: SchedulePanelComponent, resolve: { class: ClassResolver } },
        { path: 'teachers', component: TeacherManagementComponent, resolve: { teachers: TeacherManagementResolver } },
        { path: 'addTeacher', component: NewTeacherComponent },
        { path: 'editTeacher/:id', component: NewTeacherComponent, resolve: { teacher: EditTeacherResolver } },
        { path: 'teacherAssignment/:id', component: TeacherAssignmentComponent, resolve: { teacher: TeacherFormResolver } },
        { path: 'courses', component: CoursesPanelComponent, resolve: { courses: CoursesListResolver } },
        { path: 'addCourse', component: NewCourseComponent },
        { path: 'classAssignment', component: ClassStudentsAssignmentComponent },
        { path: 'courseShowing', component: CourseShowingComponent },
        { path: 'editCourse/:id', component: NewCourseComponent, resolve: { course: CourseFormResolver } },
        { path: 'levelClasses/:levelId', component: LevelClassesComponent, resolve: { classes: LevelClassesResolver } },
        { path: 'studentGrades/:id', component: GradeStudentComponent},
        { path: 'studentGradesP/:id', component: GradeStudentComponent},
        { path: 'tuitions', component: TuitionPanelComponent },
        { path: 'treasury', component: TreasuryComponent },
        { path: 'recovery', component: RecoveryComponent, resolve: { lateAmounts: RecoveryResolver }  },
        { path: 'productsList', component: ProductsListComponent, resolve: { products: ProductsListResolver } },
        { path: 'createProduct', component: ProductFormComponent },
        { path: 'editProduct/:id', component: ProductFormComponent, resolve: { product: ProductFormResolver } },
        { path: 'lvlprods', component: ClassLevelProductsComponent },
        { path: 'createlvlProduct', component: ClassLevelProdFormComponent },
        { path: 'deadLines', component: DeadLineListComponent, resolve: { deadlines: DeadLineListResolver } },
        { path: 'createDeadLine', component: DeadLineFormComponent },
        { path: 'editDeadLine/:id', component: DeadLineFormComponent, resolve: { deadline: DeadLineFormResolver } },
        { path: 'studentAgenda/:id', component: StudentAgendaComponent },
        { path: 'studentAgendaP/:id', component: StudentAgendaComponent },
        { path: 'coefficients', component: CourseCoefficientsComponent },
        { path: 'addCoefficient', component: CoefficientFormComponent },
        { path: 'editCoeffcicient/:id', component: CoefficientFormComponent, resolve: { coef: CoefficientFormResolver } },
        { path: 'periodicities', component: PeriodicitiesComponent , resolve: { periodicities: PeriodicitiesListResolver }},
        { path: 'createPeriod', component: PeriodicityFormComponent },
        { path: 'editPeriod/:id', component: PeriodicityFormComponent , resolve: { periodicity: PeriodicityFormResolver }},
        { path: 'payableAts', component: PayableAtsComponent , resolve: { payableAts: PayableAtListResolver }},
        { path: 'createPayable', component: PayableFormComponent },
        { path: 'editPayable/:id', component: PayableFormComponent , resolve: { payableAt: PayableFormResolver }},
        { path: 'createPDF', component: ConvertToPDFComponent },
        { path: 'sendSms', component: SendSmsComponent},
        { path: 'userAccount/:id', component: UserAccountComponent},
        { path: 'SmsTemplates', component: SmsTemplateComponent, resolve: { templates: SmsTemplateResolver} },
        { path: 'AddSmsTemplate', component: AddSmsTemplateComponent},
        { path: 'EditSmsTemplate/:id', component: AddSmsTemplateComponent, resolve: { template: EditSmsTemplateResolver} },
        { path: 'EmailTemplates', component: EmailTemplateComponent, resolve: { templates: EmailTemplateResolver} },
        { path: 'AddEmailTemplate', component: AddEmailTemplateComponent},
        { path: 'EditEmailTemplate/:id', component: AddEmailTemplateComponent, resolve: { template: EditEmailTemplateResolver} },
        { path: 'AddUserGrades/:evalId', component: AddUserGradesComponent, resolve: {data: ClassGradesResolver}},
        { path: 'teacherProgram', component: TeacherProgramComponent},
        { path: 'classProgram/:courseId', component: ClassProgressComponent, resolve: {program: TeacherProgramResolver}},
        { path: 'themesList', component: ThemesListComponent},
        { path: 'newTheme', component: NewThemeComponent},
        { path: 'addClassEvent/:id', component: AddClassLifeComponent},
        { path: 'checkout/:id', component: CheckoutComponent, resolve: {order: CheckoutResolver}},
        { path: 'newTuition', component: NewTuitionComponent},
        { path: 'invalidAccount', component: InvalidAccountComponent},
        { path: 'editChildren/:id', component: EditChildrenComponent, resolve: {users: EditChildrenResolver}},
        { path: 'settings', component: SchoolSettingsComponent, resolve: {settings: SchoolSettingsResolver}},
        { path: 'childFile/:id', component: ChildFileComponent, resolve: {file: ChildFileResolver} },
        { path: 'fileUser/:id', component: ChildFileComponent, resolve: {file: ChildFileResolver} },
        { path: 'parentFile/:id', component: ParentFileComponent, resolve: {file: ParentFileResolver} },
        { path: 'teacherFile/:id', component: TeacherFileComponent},
        { path: 'roles', component: RolesComponent, resolve: {roles: RolesResolver} },
        { path: 'addRole', component: AddRoleComponent},
        { path: 'editRole/:id', component: AddRoleComponent, resolve: {role: EditRoleResolver} },
        { path: 'addFinOp/:id', component: AddPaymentComponent, resolve: {file: AddPaymentResolver} },
        { path: 'validatePayments', component: ValidatePaymentsComponent, resolve: {payments: ValidatePaymentsResolver}},
        { path: 'tuitionList', component: TuitionListComponent, resolve: {list: TuitionListResolver}},
        { path: 'tuitionData', component: TuitionDataComponent},
        { path: 'tuitionDetails/:levelId', component: TuitionDetailsComponent, resolve: {users: TuitionDetailsResolver}},
        { path: 'tuitionFees', component: TuitionFeesComponent, resolve: {fees: TuitionFeesResolver}},
        { path: 'levelLatePayments', component: RecoveryListComponent},
        { path: 'childLatePayments', component: ChildrenRecoveryComponent, resolve: {parents: ChildLatePaymentsResolver}},
        { path: 'paymentLevelChild/:id', component: LevelRecoveryComponent, resolve: {parents: PaymentLevelChildResolver}},
        { path: 'absences', component: AbsencesComponent},
        { path: 'editAccount/:id', component: EditAccountComponent, resolve: {user: EditAccountResolver}},
        { path: 'usersValidation', component: UserValidationComponent, resolve: {users: UserValidationResolver} },
        { path: 'config', component: ConfigComponent},
        { path: 'editClasses/:levelId', component: EditLevelClassesComponent, resolve: { classes: LevelClassesResolver } },
        { path: 'employees', component: EmployeesComponent, resolve: {employees: EmployeesResolver}},
        { path: 'addEmployee', component: AddEmployeeComponent},
        { path: 'services', component: ServicesComponent, resolve: {services: ServicesResolver}},
        { path: 'addService/:id', component: AddServiceComponent, resolve: {product: AddServiceResolver}},
        { path: 'dueDateDetails/:duedate/:invoiced/:paid', component: DuedateDetailsComponent, resolve: {dueDate: DuedateDetailsResolver}}
      ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full' }
];
