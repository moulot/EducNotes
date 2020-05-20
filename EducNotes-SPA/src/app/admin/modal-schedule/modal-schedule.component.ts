import { Component, OnInit, Input, EventEmitter, Output, ɵConsole } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import { UserService } from 'src/app/_services/user.service';
import { User } from 'src/app/_models/user';
import { Course } from 'src/app/_models/course';
import { Schedule } from 'src/app/_models/schedule';
import { MDBModalRef } from 'ng-uikit-pro-standard';

@Component({
  selector: 'app-modal-schedule',
  templateUrl: './modal-schedule.component.html',
  styleUrls: ['./modal-schedule.component.scss']
})
export class ModalScheduleComponent implements OnInit {
  @Output() saveSchedule = new EventEmitter();
  classId: number;
  courses: Course[];
  teachers: User[];
  scheduleForm: FormGroup;
  formTouched = false;
  periodConflict = false;
  teacherSchedule: any;
  timeMask = [/\d/, /\d/, ':', /\d/, /\d/];
  agendaItems: Schedule[] = [];
  teacherOptions: any = [];
  courseOptions: any = [];
  courseConflicts: any = [];
  weekDays = ['lundi', 'mardi', 'mercredi', 'jeudi', 'vendredi', 'samedi'];
  daySelect = [{value: null, label: ''}, {value: 1, label: 'lundi'}, {value: 2, label: 'mardi'}, {value: 3, label: 'mercredi'},
    {value: 4, label: 'jeudi'}, {value: 5, label: 'vendredi'}, {value: 6, label: 'samedi'}];

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private classService: ClassService, private userService: UserService,
    public activeModal: MDBModalRef) { }

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
      }, {validator: this.item5Validator})
    }, {validator: this.formValidator});
  }

  formValidator(g: FormGroup) {
    const form1IsValid = g.controls['item1'].valid;
    const form2IsValid = g.controls['item2'].valid;
    const form3IsValid = g.controls['item3'].valid;
    const form4IsValid = g.controls['item4'].valid;
    const form5IsValid = g.controls['item5'].valid;
    const teacherIsValid = g.controls['teacher'].valid;
    const courseIsValid = g.controls['course'].valid;
    let formTouched = false;

    let item1touched = false;
    let item2touched = false;
    let item3touched = false;
    let item4touched = false;
    let item5touched = false;

    const day1touched = g.controls['item1'].get('day1').touched;
    const hourStart1touched = g.controls['item1'].get('hourStart1').touched;
    const hourEnd1touched = g.controls['item1'].get('hourEnd1').touched;
    if (day1touched || hourStart1touched || hourEnd1touched) {
      item1touched = true;
    }

    const day2touched = g.controls['item2'].get('day2').touched;
    const hourStart2touched = g.controls['item2'].get('hourStart2').touched;
    const hourEnd2touched = g.controls['item2'].get('hourEnd2').touched;
    if (day2touched || hourStart2touched || hourEnd2touched) {
      item2touched = true;
    }

    const day3touched = g.controls['item3'].get('day3').touched;
    const hourStart3touched = g.controls['item3'].get('hourStart3').touched;
    const hourEnd3touched = g.controls['item3'].get('hourEnd3').touched;
    if (day3touched || hourStart3touched || hourEnd3touched) {
      item3touched = true;
    }

    const day4touched = g.controls['item4'].get('day4').touched;
    const hourStart4touched = g.controls['item4'].get('hourStart4').touched;
    const hourEnd4touched = g.controls['item4'].get('hourEnd4').touched;
    if (day4touched || hourStart4touched || hourEnd4touched) {
      item4touched = true;
    }

    const day5touched = g.controls['item5'].get('day5').touched;
    const hourStart5touched = g.controls['item5'].get('hourStart5').touched;
    const hourEnd5touched = g.controls['item5'].get('hourEnd5').touched;
    if (day5touched || hourStart5touched || hourEnd5touched) {
      item5touched = true;
    }

    if (item1touched || item2touched || item3touched || item4touched || item5touched) {
      formTouched = true;
    }

    if (!form1IsValid || !form2IsValid || !form3IsValid || !form4IsValid ||
        !form5IsValid || !teacherIsValid || !courseIsValid || !formTouched) {
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

  isConflict(index) {
    const formIsValid = this.scheduleForm.controls['item' + index].valid;
    if (formIsValid) {
      const day = this.scheduleForm.controls['item' + index].get('day' + index).value;
      const dayIndex = this.teacherSchedule.days.findIndex(elt => elt.day === day);
      const dayCourses = this.teacherSchedule.days[dayIndex];
      if (dayCourses) {
        console.log('in if');
        const dayName = dayCourses.dayName;
        const startH = this.scheduleForm.controls['item' + index].get('hourStart' + index).value;
        const endH = this.scheduleForm.controls['item' + index].get('hourEnd' + index).value;
        this.resetConflicts(day);

        if (day || startH.length === 5 || endH.length === 5) {
          let conflict = false;
          this.periodConflict = false;
          for (let i = 0; i < dayCourses.courses.length; i++) {
            const elt = dayCourses.courses[i];
            const courseStartH = Number(elt.startH.replace(':', ''));
            const courseEndH = Number(elt.endH.replace(':', ''));
            const startHNum = Number(startH.replace(':', ''));
            const endHNum = Number(endH.replace(':', ''));
            if ((startHNum >= courseStartH && startHNum <= courseEndH) || (endHNum >= courseStartH && endHNum <= courseEndH) ||
              (startHNum < courseStartH && endHNum > courseEndH)) {
              const conflictElt = { day: day, data: 'conflit ligne 1 avec le cours du ' + dayName + '. horaire : ' + elt.startH + ' - ' + elt.endH };
              this.courseConflicts = [...this.courseConflicts, conflictElt];
              dayCourses.courses.find(c => c.courseId === elt.courseId).inConflict = true;
              conflict = true;
              this.periodConflict = true;
            }
          }
          return conflict;
        } else {
          return false;
        }
      }
    }
    return false;
  }

  resetConflicts(day) {
    const dayCourses = this.teacherSchedule.days.find(c => c.day === day);
    if (dayCourses) {
      for (let i = 0; i < dayCourses.courses.length; i++) {
        const elt = dayCourses.courses[i];
        elt.inConflict = false;
      }
    }

    if (this.courseConflicts.length > 0) {
      for (let k = this.courseConflicts.length - 1; k >= 0 ; k--) {
        const cc = this.courseConflicts[k];
        if (cc.day === day) {
          this.courseConflicts.splice(k, 1);
        }
      }
    }
  }

  saveScheduleItem() {
    for (let i = 1; i <= 5; i++) {
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
    this.saveSchedule.emit(this.agendaItems);
    this.activeModal.hide();
  }

  onTeacherChanged() {
    this.courseOptions = [];
    const teacherId = this.scheduleForm.value.teacher;
    this.getTeacherCourses(teacherId);
    this.getTeacherSchedule(teacherId);
  }

  getTeacherSchedule(teacherId) {
    this.userService.getTeacherScheduleByDay(teacherId).subscribe(data => {
      this.teacherSchedule = data;
    }, error => {
      this.alertify.error(error);
    });
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
      for (let i = 0; i < this.teachers.length; i++) {
        const elt = this.teachers[i];
        this.teacherOptions = [...this.teacherOptions, {value: elt.id, label: elt.firstName + ' ' + elt.lastName}];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  loadCourseSelect() {
    for (let i = 0; i < this.courses.length; i++) {
      const elt = this.courses[i];
      this.courseOptions = [...this.courseOptions, {value: elt.id, label: elt.name}];
    }
  }

}
