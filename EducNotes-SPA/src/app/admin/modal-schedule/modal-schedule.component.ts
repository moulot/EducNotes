import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import { UserService } from 'src/app/_services/user.service';
import { User } from 'src/app/_models/user';
import { Course } from 'src/app/_models/course';
import { Schedule } from 'src/app/_models/schedule';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-modal-schedule',
  templateUrl: './modal-schedule.component.html',
  styleUrls: ['./modal-schedule.component.scss']
})
export class ModalScheduleComponent implements OnInit {
  @Input() classId: number;
  courses: Course[];
  teachers: User[];
  timeMask = [/\d/, /\d/, ':', /\d/, /\d/];
  agendaItems: Schedule[] = [];
  scheduleForm: FormGroup;
  teacherOptions: any = [];
  courseOptions: any = [];
  weekDays = ['lundi', 'mardi', 'mercredi', 'jeudi', 'vendredi', 'samedi'];
  daySelect = [{'value': 1, 'name': 'lundi'}, {'value': 2, 'name': 'mardi'}, {'value': 3, 'name': 'mercredi'},
    {'value': 4, 'name': 'jeudi'}, {'value': 5, 'name': 'vendredi'}, {'value': 6, 'name': 'samedi'}];

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private classService: ClassService, private userService: UserService,
    public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.createScheduleForm();
    this.getClassTeachers(this.classId);
  }

  createScheduleForm() {
    this.scheduleForm = this.fb.group({
      teacher: [null, Validators.required],
      course: [null, Validators.required],
      item1: this.fb.group({
        day1: [null],
        hourStart1: [''],
        hourEnd1: ['']
      }, {validator: this.item1Validator}),
      item2: this.fb.group({
        day2: [null],
        hourStart2: [''],
        hourEnd2: ['']
      }, {validator: this.item2Validator}),
      item3: this.fb.group({
        day3: [null],
        hourStart3: [''],
        hourEnd3: ['']
      }, {validator: this.item3Validator}),
      item4: this.fb.group({
        day4: [null],
        hourStart4: [''],
        hourEnd4: ['']
      }, {validator: this.item4Validator}),
      item5: this.fb.group({
        day5: [null],
        hourStart5: [''],
        hourEnd5: ['']
      }, {validator: this.item5Validator}),
      item6: this.fb.group({
        day6: [null],
        hourStart6: [''],
        hourEnd6: ['']
      }, {validator: this.item6Validator})
    }, {validator: this.formValidator});
  }

  formValidator(g: FormGroup) {
    const form1IsValid = g.controls['item1'].valid;
    const form2IsValid = g.controls['item2'].valid;
    const form3IsValid = g.controls['item3'].valid;
    const form4IsValid = g.controls['item4'].valid;
    const form5IsValid = g.controls['item5'].valid;
    const form6IsValid = g.controls['item6'].valid;

    if (!form1IsValid || !form2IsValid || !form3IsValid || !form4IsValid || !form5IsValid || !form6IsValid) {
      return {'formNOK': true};
    }

    return null;
  }

  item1Validator(g: FormGroup) {
    // line 1
    const day1 = g.get('day1').value;
    const hourStart1 = g.get('hourStart1').value;
    const hourEnd1 = g.get('hourEnd1').value;
    if (day1 !== null || hourStart1.length > 0 || hourEnd1.length > 0) {
      const typedHS1 = hourStart1.replace('_', '');
      const typedHE1 = hourEnd1.replace('_', '');
      if (day1 !== null && typedHS1.length > 1 && typedHE1.length > 1) {
        if (typedHS1.length !== 5 || typedHE1.length !== 5) {
          return {'line1DatesNOK': true};
        } else {
          return null;
        }
      } else {
        return {'line1NOK': true};
      }
    }
  }

  item2Validator(g: FormGroup) {
    // line 2
    const day2 = g.get('day2').value;
    const hourStart2 = g.get('hourStart2').value;
    const hourEnd2 = g.get('hourEnd2').value;
    if (day2 !== null || hourStart2.length > 0 || hourEnd2.length > 0) {
      const typedHS2 = hourStart2.replace('_', '');
      const typedHE2 = hourEnd2.replace('_', '');
      if (day2 !== null && typedHS2.length > 1 && typedHE2.length > 1) {
        if (typedHS2.length !== 5 || typedHE2.length !== 5) {
          return {'line2DatesNOK': true};
        } else {
          return null;
        }
      } else {
        return {'line2NOK': true};
      }
    }
  }

  item3Validator(g: FormGroup) {
    // line 3
    const day3 = g.get('day3').value;
    const hourStart3 = g.get('hourStart3').value;
    const hourEnd3 = g.get('hourEnd3').value;
    if (day3 !== null || hourStart3.length > 0 || hourEnd3.length > 0) {
      if (day3 !== null && hourStart3.length > 0 && hourEnd3.length > 0) {
        const typedHS3 = hourStart3.replace('_', '');
        const typedHE3 = hourEnd3.replace('_', '');
        if (typedHS3.length !== 5 || typedHE3.length !== 5) {
          return {'line3DatesNOK': true};
        } else {
          return null;
        }
      } else {
        return {'line3NOK': true};
      }
    }
  }

  item4Validator(g: FormGroup) {
    // line 4
    const day4 = g.get('day4').value;
    const hourStart4 = g.get('hourStart4').value;
    const hourEnd4 = g.get('hourEnd4').value;
    if (day4 !== null || hourStart4.length > 0 || hourEnd4.length > 0) {
      if (day4 !== null && hourStart4.length > 0 && hourEnd4.length > 0) {
        const typedHS4 = hourStart4.replace('_', '');
        const typedHE4 = hourEnd4.replace('_', '');
        if (typedHS4.length !== 5 || typedHE4.length !== 5) {
          return {'line4DatesNOK': true};
        } else {
          return null;
        }
      } else {
        return {'line4NOK': true};
      }
    }
  }

  item5Validator(g: FormGroup) {
    // line 5
    const day5 = g.get('day5').value;
    const hourStart5 = g.get('hourStart5').value;
    const hourEnd5 = g.get('hourEnd5').value;
    if (day5 !== null || hourStart5.length > 0 || hourEnd5.length > 0) {
      if (day5 !== null && hourStart5.length > 0 && hourEnd5.length > 0) {
        const typedHS5 = hourStart5.replace('_', '');
        const typedHE5 = hourEnd5.replace('_', '');
        if (typedHS5.length !== 5 || typedHE5.length !== 5) {
          return {'line5DatesNOK': true};
        } else {
          return null;
        }
      } else {
        return {'line5NOK': true};
      }
    }
  }

  item6Validator(g: FormGroup) {
    // line 6
    const day6 = g.get('day6').value;
    const hourStart6 = g.get('hourStart6').value;
    const hourEnd6 = g.get('hourEnd6').value;
    if (day6 !== null || hourStart6.length > 0 || hourEnd6.length > 0) {
      if (day6 !== null && hourStart6.length > 0 && hourEnd6.length > 0) {
        const typedHS6 = hourStart6.replace('_', '');
        const typedHE6 = hourEnd6.replace('_', '');
        if (typedHS6.length !== 5 || typedHE6.length !== 5) {
          return {'line6DatesNOK': true};
        } else {
          return null;
        }
      } else {
        return {'line6NOK': true};
      }
    }
  }

  saveScheduleItem() {
    for (let i = 1; i <= 6; i++) {
      // is the schedule item line empty?
      if (this.scheduleForm.controls['item' + i].get('day' + i).value !== null) {
        const sch = <Schedule>{};
        sch.classId = this.classId;
        sch.teacherId = this.scheduleForm.value.teacher;
        sch.courseId = this.scheduleForm.value.course;
        sch.day = this.scheduleForm.controls['item' + i].get('day' + i).value;
        const hStart = this.scheduleForm.controls['item' + i].get('hourStart' + i).value.split(':');
        const hEnd = this.scheduleForm.controls['item' + i].get('hourEnd' + i).value.split(':');
        const hourStart = new Date(2019, 0, 1, hStart[0], hStart[1]);
        const hourEnd = new Date(2019, 0, 1, hEnd[0], hEnd[1]);
        sch.startHourMin = hourStart;
        sch.endHourMin = hourEnd;
        this.agendaItems = [...this.agendaItems, sch];
        }
    }
    console.log(this.agendaItems);
    this.activeModal.close(this.agendaItems);
  }

  closeModal() {
    this.activeModal.dismiss();
  }

  onTeacherChanged() {
    const teacherId = this.scheduleForm.value.teacher;
    this.getTeacherCourses(teacherId);
  }

  getTeacherCourses(teacherId) {
    this.userService.getTeacherCourses(teacherId).subscribe((courses: Course[]) => {
      this.courses = courses;
      this.loadCourseSelect();
    }, error => {
      this.alertify.error(error);
    });
  }

  getClassTeachers(classId) {
    this.classService.getClassTeachers(classId).subscribe((teachers: User[]) => {
      this.teachers = teachers;
      this.loadTeacherSelect();
    }, error => {
      this.alertify.error(error);
    });
  }

  loadTeacherSelect() {
    for (let i = 0; i < this.teachers.length; i++) {
      const elt = this.teachers[i];
      this.teacherOptions = [...this.teacherOptions, {value: elt.id, label: elt.firstName + ' ' + elt.lastName}];
    }
  }

  loadCourseSelect() {
    for (let i = 0; i < this.courses.length; i++) {
      const elt = this.courses[i];
      this.courseOptions = [...this.courseOptions, {value: elt.id, label: elt.name}];
    }
  }

}
