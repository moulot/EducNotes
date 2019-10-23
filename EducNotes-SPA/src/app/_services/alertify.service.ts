import { Injectable } from '@angular/core';
// import { ToastrService } from 'ngx-toastr';
import { ToastService } from 'ng-uikit-pro-standard';

declare let alertify: any;

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

  // constructor(private toastrService: ToastrService) { }
  constructor(private toastrService: ToastService) { }

  confirm(message: string, okCallback: () => any) {
    alertify.confirm(message, function(e) {
      if (e) {
        okCallback();
      } else {}
    });
  }
  // success(message: string) {
  //   alertify.success(message);
  // }

  // error(message: string) {
  //   alertify.error(message);
  // }

  // warning(message: string) {
  //   alertify.warning(message);
  // }

  success(message: string) {
    this.toastrService.success(message, 'Educ\'Notes', { timeOut: 3000 });
  }
  warning(message: string) {
    this.toastrService.warning(message, 'Educ\'Notes', { timeOut: 3000 });
  }
  info(message: string) {
    this.toastrService.info(message, 'Educ\'Notes', { timeOut: 3000 });
  }
  error(message: string) {
    this.toastrService.error(message, 'Educ\'Notes', { timeOut: 3000 });
  }

  successBar(message: string) {
    this.toastrService.success(message, 'Educ\'Notes', { timeOut: 3000, closeButton: true, progressBar: true,
      positionClass: 'toast-bottom-right' });
  }
  warningBar(message: string) {
    this.toastrService.warning(message, 'Educ\'Notes', { timeOut: 3000, closeButton: true, progressBar: true });
  }
  infoBar(message: string) {
    this.toastrService.info(message, 'Educ\'Notes', { timeOut: 3000, closeButton: true, progressBar: true });
  }
  errorBar(message: string) {
    this.toastrService.error(message, 'Toastr title', { timeOut: 3000, closeButton: true, progressBar: true });
  }

}
