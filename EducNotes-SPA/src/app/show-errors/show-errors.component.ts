import { Component, OnInit, Input } from '@angular/core';
import { AbstractControlDirective, AbstractControl } from '@angular/forms';

@Component({
  selector: 'app-show-errors',
  templateUrl: './show-errors.component.html',
  styleUrls: ['./show-errors.component.css']
})
export class ShowErrorsComponent implements OnInit {

  private static readonly errorMessages = {
    'required': () => 'This field is required',
    'minlength': (params) => 'The min number of characters is ' + params.requiredLength,
    'maxlength': (params) => 'The max allowed number of characters is ' + params.requiredLength,
    'pattern': (params) => 'The required pattern is: ' + params.requiredPattern,
    'years': (params) => params.message,
    'isGradedMaxGrade': (params) => params.message,
    'uniqueName': (params) => params.message,
    'telephoneNumber': (params) => params.message
  };

  @Input() private control: AbstractControlDirective | AbstractControl;
  test = false;

  constructor() { }

  ngOnInit() {
  }

  shouldShowErrors(): boolean {
    console.log('in fct' + ' - errors: ' + this.control.errors + ' - dirty:' + this.control.dirty +
      ' - touched:' + this.control.touched);
    this.test = this.control && (this.control.dirty || this.control.touched);
    return this.test;
  }

  listOfErrors(): string[] {
    console.log('in listOfErrors fct- ctrl: ' + this.control.errors);
    return Object.keys(this.control.errors).map(field => this.getMessage(field, this.control.errors[field]));
  }

  private getMessage(type: string, params: any) {
    return ShowErrorsComponent.errorMessages[type](params);
  }

}
