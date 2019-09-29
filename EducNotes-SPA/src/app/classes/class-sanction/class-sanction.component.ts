import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/_models/user';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserSanction } from 'src/app/_models/userSanction';
import { Sanction } from 'src/app/_models/sanction';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-class-sanction',
  templateUrl: './class-sanction.component.html',
  styleUrls: ['./class-sanction.component.css']
})
export class ClassSanctionComponent implements OnInit {
  @Input() students: User[];
  @Input() selectedClass: any;
  @Output() toggleForm = new EventEmitter<boolean>();
  @Output() loadSanctions = new EventEmitter<any>();
  sanctionForm: FormGroup;
  classTeachers: User[];
  newSanction = <UserSanction>{};
  sanctions = <Sanction>{};

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private userService: UserService, private fb: FormBuilder) { }

  ngOnInit() {
    this.createSanctionForm();
    this.getClassTeachers(this.selectedClass.id);
    this.getSanctions();
  }

  createSanctionForm() {
    this.sanctionForm = this.fb.group({
      student: [null, Validators.required],
      sanction: [null, Validators.required],
      date: [null, Validators.required],
      sanctionedBy: [null, Validators.required],
      reason: ['', Validators.required],
      comment: ['']
    });
  }

  getClassTeachers(classId) {
    this.classService.getClassTeachers(classId).subscribe(teachers => {
      this.classTeachers = teachers;
    }, error => {
      this.alertify.error(error);
    });
  }

  getSanctions() {
    this.classService.getSanctions().subscribe((sanctions: Sanction) => {
      this.sanctions = sanctions;
    });
  }

  saveSanction(more: boolean) {
    this.newSanction.userId = this.sanctionForm.value.student;
    this.newSanction.sanctionId = this.sanctionForm.value.sanction;
    this.newSanction.sanctionedById = this.sanctionForm.value.sanctionedBy;
    this.newSanction.sanctionDate = this.sanctionForm.value.date;
    this.newSanction.reason = this.sanctionForm.value.reason;
    this.newSanction.comment = this.sanctionForm.value.comment;

    this.userService.saveSanction(this.newSanction).subscribe(() => {
      this.alertify.success('ajout de la sanction validé.');
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.sanctionForm.reset();
      if (more === false) {
        this.toggleForm.emit();
        this.loadSanctions.emit();
      }
    });
  }

  cancel() {
    this.sanctionForm.reset();
  }

}
