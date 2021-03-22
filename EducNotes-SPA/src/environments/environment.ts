// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api/',
  tuitionId: 1,
  nextYearTuitionId: 2,
  studentTypeId: 1,
  teacherTypeId: 2,
  parentTypeId: 3,
  adminTypeId: 4,
  absenceType: 1,
  lateType: 2,
  schoolServiceId: 1,
  maxChildNumber: 6,
  byDeadLineTypeId: true,
  payCash: 1,
  payCheque: 2,
  payWire: 3,
  payMobile: 4,
  finOpTypeInvoice: 1,
  finOpTypePayment: 2,
  educLevelPrimary: 1,
  educLevelSecondary: 2,
  terminalCycle: 5
};

/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
