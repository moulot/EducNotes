import { Directive, HostListener } from '@angular/core';
import { Location } from '@angular/common';
@Directive({
  selector: '[appBtnBack]'
})
export class BtnBackDirective {

  constructor(private location: Location) { }

  @HostListener('click')
  onClick() {
      this.location.back();
  }
}
