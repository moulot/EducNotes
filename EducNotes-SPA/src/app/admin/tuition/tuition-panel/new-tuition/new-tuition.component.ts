import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-new-tuition',
  templateUrl: './new-tuition.component.html',
  styleUrls: ['./new-tuition.component.scss']
})
export class NewTuitionComponent implements OnInit {
  tuitionForm: FormGroup;
  levels: any;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private classService: ClassService) { }

  ngOnInit() {
    this.createTuitionForm();
  }

  getClassLevels() {
    this.classService.getLevels().subscribe(data => {
      this.levels = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  createTuitionForm() {
    this.tuitionForm = this.fb.group({
      fLastName: [''],
      fFirstName: [''],
      fEmail: [''],
      fCell: [''],
      mLastName: [''],
      mFirstName: [''],
      mEmail: [''],
      mCell: [''],
      children: this.fb.array([])
    });
  }

  addChildrenItem(lname, fname, classlevel): void {
    const children = this.tuitionForm.get('children') as FormArray;
    children.push(this.createChildrenItem(lname, fname, classlevel));
  }

  createChildrenItem(lname, fname, classlevel): FormGroup {
    return this.fb.group({
      lname: lname,
      fname: fname,
      classlevel: classlevel
    });
  }

  saveTuition() {

  }

}
