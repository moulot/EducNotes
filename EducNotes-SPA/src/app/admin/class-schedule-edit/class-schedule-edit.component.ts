import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Course } from 'src/app/_models/course';
import { ScheduleData } from 'src/app/_models/scheduleData';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import { UserService } from 'src/app/_services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-class-schedule-edit',
  templateUrl: './class-schedule-edit.component.html',
  styleUrls: ['./class-schedule-edit.component.scss']
})
export class ClassScheduleEditComponent implements OnInit {
  educLevelPrimary = environment.educLevelPrimary;
  classId: number;
  class: any;
  courses: Course[];
  teachers: User[];
  teacherName: string;
  scheduleForm: FormGroup;
  formTouched = false;
  periodConflict = false;
  teacherSchedule: any;
  scheduleCourses: any;
  timeMask = [/\d/, /\d/, ':', /\d/, /\d/];
  agendaItems: ScheduleData[] = [];
  teacherOptions: any = [];
  courseOptions: any = [];
  courseConflicts: any = [];
  weekDays = ['lundi', 'mardi', 'mercredi', 'jeudi', 'vendredi'];
  daySelect = [{value: null, label: ''}, {value: 1, label: 'lundi'}, {value: 2, label: 'mardi'}, {value: 3, label: 'mercredi'},
    {value: 4, label: 'jeudi'}, {value: 5, label: 'vendredi'}]; // , {value: 6, label: 'samedi'}];
  dayItems = [];
  monCourses = [];
  tueCourses = [];
  wedCourses = [];
  thuCourses = [];
  friCourses = [];
  satCourses = [];
  sunCourses = [];
  wait = false;

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private route: ActivatedRoute,
    private classService: ClassService, private userService: UserService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.classId = params['classId'];
      this.getClass(this.classId);
      this.getClassTeachers(this.classId);
      this.getScheduleCourses(this.classId);
    });
    this.createScheduleForm();
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
    console.log(day1 + '-' + hourStart1 + '-' + hourEnd1);
    if (day1 != null) {
      if (hourStart1.length > 0 || hourEnd1.length > 0) {
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
    this.courseConflicts = [];
    const formIsValid = this.scheduleForm.controls['item' + index].valid;
    if (formIsValid) {
      const day = this.scheduleForm.controls['item' + index].get('day' + index).value;
      const startH = this.scheduleForm.controls['item' + index].get('hourStart' + index).value;
      const endH = this.scheduleForm.controls['item' + index].get('hourEnd' + index).value;
      const startHNum = Number(startH.replace(':', ''));
      const endHNum = Number(endH.replace(':', ''));
      // cope with teacher schedule
      let teacherDayCourses = [];
      if (this.teacherSchedule.days) {
        const teacherDayIndex = this.teacherSchedule.days.findIndex(elt => elt.day === day);
        if (teacherDayIndex !== -1) {
          teacherDayCourses = [...teacherDayCourses, this.teacherSchedule.days[teacherDayIndex]];
        }
      }

      // then with class schedule (remove courses already treated with teacher)
      let classDayCourses = <any>{};
      if (this.scheduleCourses.days) {
        const classDayIndex = this.scheduleCourses.days.findIndex(elt => elt.day === day);
        classDayCourses = classDayIndex !== -1 ? this.scheduleCourses.days[classDayIndex] : [];
      }

      let filteredClassDayCourses = [];
      if (classDayCourses.length > 0) {
        if (teacherDayCourses.length > 0) {
          for (let i = 0; i < classDayCourses.courses.length; i++) {
            const elt = classDayCourses.courses[i];
            if (teacherDayCourses.length > 0) {
              for (let j = 0; j < teacherDayCourses.length; j++) {
                const elt1 = teacherDayCourses[j];
                for (let k = 0; k < elt1.courses.length; k++) {
                  const course = elt1.courses[k];
                  const courseIndex = teacherDayCourses.findIndex(c => c.courseId === elt.courseId);
                  if (courseIndex === -1) {
                    filteredClassDayCourses = [...filteredClassDayCourses, elt];
                  }
                }
              }
            }
          }
        } else {
          filteredClassDayCourses = [...filteredClassDayCourses, classDayCourses];
        }
      } else {
        filteredClassDayCourses = [...filteredClassDayCourses, classDayCourses];
      }

      const classDayName = classDayCourses.dayName;
      this.resetConflicts(day);

      // let conflict = false;
      this.periodConflict = false;

      if (teacherDayCourses.length > 0) {
        if (day || startH.length === 5 || endH.length === 5) {
          for (let i = 0; i < teacherDayCourses.length; i++) {
            const elt = teacherDayCourses[i];
            for (let j = 0; j < elt.courses.length; j++) {
              const course = elt.courses[j];
              const courseStartH = Number(course.startH.replace(':', ''));
              const courseEndH = Number(course.endH.replace(':', ''));
              if ((startHNum >= courseStartH && startHNum <= courseEndH) || (endHNum >= courseStartH &&
                endHNum <= courseEndH) || (startHNum < courseStartH && endHNum > courseEndH)) {
                const conflictElt = { day: day, data: 'ligne ' + index + '. conflit avec le cours ' + course.courseAbbrev +
                  ' du ' + this.teacherName + '. horaire : ' + course.startH + ' - ' + course.endH };
                this.courseConflicts = [...this.courseConflicts, conflictElt];
                teacherDayCourses[i].courses[j].inConflict = true;
                // conflict = true;
                this.periodConflict = true;
              }
            }
          }
        }
      }

      if (filteredClassDayCourses.length > 0) {
        if (day || startH.length === 5 || endH.length === 5) {
          for (let i = 0; i < filteredClassDayCourses.length; i++) {
            const elt = filteredClassDayCourses[i];
            if (elt.courses) {
              for (let j = 0; j < elt.courses.length; j++) {
                const course = elt.courses[j];
                const courseStartH = Number(course.startH.replace(':', ''));
                const courseEndH = Number(course.endH.replace(':', ''));
                if ((startHNum >= courseStartH && startHNum <= courseEndH) || (endHNum >= courseStartH &&
                  endHNum <= courseEndH) || (startHNum < courseStartH && endHNum > courseEndH)) {
                  const conflictElt = { day: day, data: 'ligne classe ' + index + '. conflit avec le cours ' +
                  course.courseAbbrev + ' du ' + classDayName + '. horaire : ' + course.startH + ' - ' + course.endH };
                  this.courseConflicts = [...this.courseConflicts, conflictElt];
                  filteredClassDayCourses[i].courses[j].inConflict = true;
                  this.periodConflict = true;
                }
              }
            }
          }
        }
      }
    } else {
      this.courseConflicts = [];
    }
  }

  resetConflicts(day) {
    if (this.teacherSchedule.days) {
      const dayCourses = this.teacherSchedule.days.find(c => c.day === day);
      if (dayCourses) {
        for (let i = 0; i < dayCourses.courses.length; i++) {
          const elt = dayCourses.courses[i];
          elt.inConflict = false;
        }
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
    this.agendaItems = [];
    for (let i = 1; i <= 5; i++) {
      // is the schedule item line empty?
      if (this.scheduleForm.controls['item' + i].get('day' + i).value !== null) {
        const sch = <ScheduleData>{};
        sch.classId = this.classId;
        sch.teacherId = this.scheduleForm.value.teacher;
        sch.courseId = this.scheduleForm.value.course;
        sch.day = this.scheduleForm.controls['item' + i].get('day' + i).value;
        const hStart = this.scheduleForm.controls['item' + i].get('hourStart' + i).value.split(':');
        const hEnd = this.scheduleForm.controls['item' + i].get('hourEnd' + i).value.split(':');
        sch.startHour = hStart[0];
        sch.startMin = hStart[1];
        sch.endHour = hEnd[0];
        sch.endMin = hEnd[1];
        this.agendaItems = [...this.agendaItems, sch];
      }
    }
    this.saveSchedule(this.agendaItems);
  }

  saveSchedule(schedules: ScheduleData[]) {
    this.classService.saveSchedules(schedules).subscribe(() => {
      const teacherId = this.scheduleForm.value.teacher;
      this.getTeacherSchedule(teacherId);
      this.getScheduleCourses(this.classId);
      this.resetForm();
      this.alertify.success('cours de l\'emploi du temps enregistrés');
    }, error => {
      this.alertify.error(error);
    });
  }

  resetForm() {
    this.scheduleForm.reset();
    this.scheduleForm.controls['item1'].reset();
    this.scheduleForm.controls['item2'].reset();
    this.scheduleForm.controls['item3'].reset();
    this.scheduleForm.controls['item4'].reset();
    this.scheduleForm.controls['item5'].reset();
    this.scheduleForm.get('course').setValue('');
  }

  onTeacherChanged(selected) {
    this.courseConflicts = [];
    this.courseOptions = [];
    this.teacherName = selected.label;
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

  getScheduleCourses(classId) {
    this.resetSchedule();
    this.classService.getScheduleCoursesByDay(classId).subscribe((data: any) => {
      this.scheduleCourses = data;
      // add courses on the schedule
      if (this.scheduleCourses.days) {
        for (let i = 1; i <= 7; i++) {
          const filtered = this.scheduleCourses.days.filter(item => item.day === i);
          if (filtered.length > 0) {
            switch (i) {
              case 1:
                this.monCourses.push(filtered[0].courses);
                break;
              case 2:
                this.tueCourses.push(filtered[0].courses);
                break;
              case 3:
                this.wedCourses.push(filtered[0].courses);
                break;
              case 4:
                this.thuCourses.push(filtered[0].courses);
                break;
              case 5:
                this.friCourses.push(filtered[0].courses);
                break;
              case 6:
                this.satCourses.push(filtered[0].courses);
                break;
              case 7:
                this.sunCourses.push(filtered[0].courses);
                break;
              default:
                break;
            }
          }
        }
      }
      this.dayItems = [this.monCourses, this.tueCourses, this.wedCourses, this.thuCourses, this.friCourses, this.satCourses];
    }, error => {
      this.alertify.error(error);
    });
  }

  resetSchedule() {
    this.monCourses = [];
    this.tueCourses = [];
    this.wedCourses = [];
    this.thuCourses = [];
    this.friCourses = [];
    this.satCourses = [];
    this.sunCourses = [];
  }

  getClassTeachers(classId) {
    this.classService.getClassTeachers(classId).subscribe((teachers: User[]) => {
      this.teachers = teachers;
      for (let i = 0; i < this.teachers.length; i++) {
        const elt = this.teachers[i];
        this.teacherOptions = [...this.teacherOptions, {value: elt.id, label: elt.lastName + ' ' + elt.firstName}];
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

  getClass(classid) {
    this.classService.getClass(classid).subscribe(data => {
      this.class = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  goBackToSchedule() {
  }

}
