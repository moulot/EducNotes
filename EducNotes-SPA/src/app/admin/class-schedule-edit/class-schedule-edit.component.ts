import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
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
  classEducLevel: number;
  courses: any = [];
  activities: any = [];
  teachers: User[];
  teacherName: string;
  scheduleForm: FormGroup;
  conflictForm: FormGroup;
  formTouched = false;
  periodConflict = false;
  teacherSchedule: any;
  scheduleCourses: any;
  coursesByDay: any;
  oldCourseOptions: any = [];
  timeMask = [/\d/, /\d/, ':', /\d/, /\d/];
  scheduleItems: ScheduleData[] = [];
  teacherOptions: any = [];
  courseOptions: any = [];
  courseConflicts: any = [];
  validateConflict: any = [false, false, false, false, false];
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
  showConflictDiv = false;
  oldCourseColor: any;
  oldCourseInfo = 'N/D';
  conflictCourseColor: any;
  conflictCourseInfo = 'N/D';
  conflictDay: any;
  oldConflictStartHM: any;
  oldConflictEndHM: any;
  conflictStartHM: any;
  conflictEndHM: any;
  showConflictInfo = false;
  editPeriodDiv = false;
  periodChanged = false;
  conflictData: any = [];

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private route: ActivatedRoute,
    private classService: ClassService, private userService: UserService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.classId = params['classId'];
      this.getClass(this.classId);
      this.getClassTeachers(this.classId);
      this.getScheduleCourses(this.classId);
      this.getCoursesByDay(this.classId);
    });
    this.createScheduleForm();
    this.createConflictForm();
  }

  createConflictForm() {
    this.conflictForm = this.fb.group({
      oldCourse: [null],
      conflictCourse: [null],
      startHM: [],
      endHM: []
    }, {validator: this.conflictValidator});
  }

  conflictValidator(g: FormGroup) {
    const startHM = g.get('startHM').value;
    const endHM = g.get('endHM').value;
    if (startHM || endHM) {
      const typedHS = startHM ? startHM.replace('_', '') : '';
      const typedHE = endHM ? endHM.replace('_', '') : '';
      if (typedHS.length > 1 && typedHE.length > 1) {
        if (typedHS.length !== 5 || typedHE.length !== 5) {
          return {'datesNOK': true};
        } else {
          return null;
        }
      } else {
        return {'lineNOK': true};
      }
    }
  }

  createScheduleForm() {
    this.scheduleForm = this.fb.group({
      teacher: [null, Validators.required],
      course: [null, Validators.required],
      item1: this.fb.group({
        day1: [null],
        hourStart1: [null],
        hourEnd1: [null],
        conflictOK1: false
      }, {validator: this.item1Validator}),
      item2: this.fb.group({
        day2: [null],
        hourStart2: [''],
        hourEnd2: [''],
        conflictOK2: false
      }, {validator: this.item2Validator}),
      item3: this.fb.group({
        day3: [null],
        hourStart3: [''],
        hourEnd3: [''],
        conflictOK3: false
      }, {validator: this.item3Validator}),
      item4: this.fb.group({
        day4: [null],
        hourStart4: [''],
        hourEnd4: [''],
        conflictOK4: false
      }, {validator: this.item4Validator}),
      item5: this.fb.group({
        day5: [null],
        hourStart5: [''],
        hourEnd5: [''],
        conflictOK5: false
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

    // console.log(form1IsValid + '-' + form2IsValid + '-' + form3IsValid + '-' + form4IsValid + '-' + form5IsValid + ' t: ' + formTouched);
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
    if (day1) {
      if (hourStart1 || hourEnd1) {
        const typedHS1 = hourStart1 ? hourStart1.replace('_', '') : '';
        const typedHE1 = hourEnd1 ? hourEnd1.replace('_', '') : '';
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
    if (day2) {
      if (hourStart2 || hourEnd2) {
        const typedHS2 = hourStart2 ? hourStart2.replace('_', '') : '';
        const typedHE2 = hourEnd2 ? hourEnd2.replace('_', '') : '';
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
  }

  item3Validator(g: FormGroup) {
    // line 3
    const day3 = g.get('day3').value;
    const hourStart3 = g.get('hourStart3').value;
    const hourEnd3 = g.get('hourEnd3').value;
    if (day3) {
      if (hourStart3 || hourEnd3) {
        const typedHS3 = hourStart3 ? hourStart3.replace('_', '') : '';
        const typedHE3 = hourEnd3 ? hourEnd3.replace('_', '') : '';
        if (day3 !== null && typedHS3.length > 1 && typedHS3.length > 1) {
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
  }

  item4Validator(g: FormGroup) {
    // line 4
    const day4 = g.get('day4').value;
    const hourStart4 = g.get('hourStart4').value;
    const hourEnd4 = g.get('hourEnd4').value;
    if (day4) {
      if (hourStart4 || hourEnd4) {
        const typedHS4 = hourStart4 ? hourStart4.replace('_', '') : '';
        const typedHE4 = hourEnd4 ? hourEnd4.replace('_', '') : '';
        if (day4 !== null && typedHS4.length > 1 && typedHS4.length > 1) {
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
  }

  item5Validator(g: FormGroup) {
    // line 5
    const day5 = g.get('day5').value;
    const hourStart5 = g.get('hourStart5').value;
    const hourEnd5 = g.get('hourEnd5').value;
    if (day5) {
      if (hourStart5 || hourEnd5) {
        const typedHS5 = hourStart5 ? hourStart5.replace('_', '') : '';
        const typedHE5 = hourEnd5 ? hourEnd5.replace('_', '') : '';
        if (day5 !== null && typedHS5.length > 1 && typedHS5.length > 1) {
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
  }

  isConflict(index) {
    this.courseConflicts = [];
    const formIsValid = this.scheduleForm.controls['item' + index].valid;
    const day = this.scheduleForm.controls['item' + index].get('day' + index).value;
    if (formIsValid && day) {
      const startH = this.scheduleForm.controls['item' + index].get('hourStart' + index).value;
      const endH = this.scheduleForm.controls['item' + index].get('hourEnd' + index).value;
      if (startH && endH) {
        const startHNum = Number(startH.replace(':', ''));
        const endHNum = Number(endH.replace(':', ''));

        // cope with teacher schedule
        let teacherDayCourses = [];
        // console.log('teacher days');
        // console.log(this.teacherSchedule.days);
        if (this.teacherSchedule.days) {
          const teacherDayIndex = this.teacherSchedule.days.findIndex(elt => elt.day === day);
          if (teacherDayIndex !== -1) {
            teacherDayCourses = [...teacherDayCourses, this.teacherSchedule.days[teacherDayIndex]];
          }
        }
        // console.log('teacher courses');
        // console.log(teacherDayCourses);

        // then with class schedule (remove courses already treated with teacher)
        let classDayCourses = <any>{};
        if (this.coursesByDay.days) {
          const classDayIndex = this.coursesByDay.days.findIndex(elt => elt.day === day);
          classDayCourses = classDayIndex !== -1 ? this.coursesByDay.days[classDayIndex] : [];
        }
        // console.log('classes courses');
        // console.log(classDayCourses);

        // remove courses from classDayCourses that are already in teacherDayCourses
        let filteredClassDayCourses = [];
        if (classDayCourses.courses) {
          if (teacherDayCourses.length > 0) {
            for (let i = 0; i < classDayCourses.courses.length; i++) {
              const elt = classDayCourses.courses[i];
              const courseIndex = teacherDayCourses.findIndex(c => c.courseId === elt.courseId);
              if (courseIndex === -1) {
                filteredClassDayCourses = [...filteredClassDayCourses, elt];
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

        let conflict = false;
        this.periodConflict = false;
        const conflictIsOK = this.scheduleForm.controls['item' + index].get('conflictOK' + index).value;
        if (conflictIsOK === false) {
          // remove conflict data for the current line
          const conflictIndex = this.conflictData.findIndex(c => c.line === index);
          if (conflictIndex !== -1) {
            this.conflictData.splice(conflictIndex, 1);
          }
        }

        if (teacherDayCourses.length > 0) {
          for (let i = 0; i < teacherDayCourses.length; i++) {
            const elt = teacherDayCourses[i];
            for (let j = 0; j < elt.courses.length; j++) {
              const course = elt.courses[j];
              const courseStartH = Number(course.startH.replace(':', ''));
              const courseEndH = Number(course.endH.replace(':', ''));
              if ((startHNum >= courseStartH && startHNum <= courseEndH) || (endHNum >= courseStartH &&
                endHNum <= courseEndH) || (startHNum < courseStartH && endHNum > courseEndH)) {
                // console.log('teacher');
                // console.log(course);
                conflict = true;
                if (!conflictIsOK) {
                  const conflictElt = { day: day, data: 'ligne ' + index + '. conflit avec le programme de  ' + this.teacherName +
                  '. cours : ' + course.courseAbbrev + ' du ' + classDayName + ' de ' + course.startH + ' à ' + course.endH };
                  this.courseConflicts = [...this.courseConflicts, conflictElt];
                  this.validateConflict[index - 1] = true;
                  // console.log('teacher index-1: ' + (index - 1));
                  // console.log(this.validateConflict);
                  teacherDayCourses[i].courses[j].inConflict = true;
                  this.periodConflict = true;
                } else {
                  // set conflict data
                  const conflictdata = <any>{};
                  conflictdata.line = index;
                  conflictdata.id = course.id;
                  conflictdata.scheduleId = course.scheduleId;
                  conflictdata.conflictedCourseId = course.courseId;
                  this.conflictData = [...this.conflictData, conflictdata];
                }
              }
            }
          }
          if (!conflict) {
            this.validateConflict[index - 1] = false;
            this.scheduleForm.controls['item' + index].get('conflictOK' + index).setValue(false);
          }
          // console.log(this.conflictData);
        }

        if (filteredClassDayCourses.length > 0) {
          for (let i = 0; i < filteredClassDayCourses.length; i++) {
            const elt = filteredClassDayCourses[i];
            if (elt.courses) {
              for (let j = 0; j < elt.courses.length; j++) {
                const course = elt.courses[j];
                  const courseStartH = Number(course.startH.replace(':', ''));
                const courseEndH = Number(course.endH.replace(':', ''));
                if ((startHNum >= courseStartH && startHNum <= courseEndH) || (endHNum >= courseStartH &&
                  endHNum <= courseEndH) || (startHNum < courseStartH && endHNum > courseEndH)) {
                  // console.log('class');
                  // console.log(course);
                  conflict = true;
                  if (!conflictIsOK) {
                    const conflictElt = { day: day, data: 'ligne ' + index + '. conflit dans l\'emploi du temps de la classe avec le cours ' +
                    course.courseAbbrev + ' du ' + classDayName + ' de ' + course.startH + ' à ' + course.endH };
                    this.courseConflicts = [...this.courseConflicts, conflictElt];
                    this.validateConflict[index - 1] = true;
                    // console.log('class-index-1: ' + (index - 1));
                    // console.log(this.validateConflict);
                    filteredClassDayCourses[i].courses[j].inConflict = true;
                    this.periodConflict = true;
                    // remove conflict data for the current line
                    // const conflictIndex = this.conflictData.findIndex(c => c.line === index);
                    // if (conflictIsOK !== -1) {
                    //   this.conflictData.splice(conflictIndex, 1);
                    // }
                  } else {
                    // set conflict data
                    const conflictdata = <any>{};
                    conflictdata.line = index;
                    conflictdata.id = course.id;
                    conflictdata.scheduleId = course.scheduleId;
                    conflictdata.conflictedCourseId = course.courseId;
                    this.conflictData = [...this.conflictData, conflictdata];
                  }
                }
              }
            }
          }
          if (!conflict) {
            this.validateConflict[index - 1] = false;
            this.scheduleForm.controls['item' + index].get('conflictOK' + index).setValue(false);
          }
          // console.log(this.conflictData);
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

  resetForm() {
    // this.scheduleForm.reset();
    this.scheduleForm.get('course').setValue('');
    this.scheduleForm.controls['item1'].reset();
    this.scheduleForm.controls['item2'].reset();
    this.scheduleForm.controls['item3'].reset();
    this.scheduleForm.controls['item4'].reset();
    this.scheduleForm.controls['item5'].reset();
    this.scheduleForm.controls['item1'].markAsUntouched();
    this.scheduleForm.controls['item2'].markAsUntouched();
    this.scheduleForm.controls['item3'].markAsUntouched();
    this.scheduleForm.controls['item4'].markAsUntouched();
    this.scheduleForm.controls['item5'].markAsUntouched();
    this.validateConflict = [false, false, false, false, false];
  }

  onTeacherChanged(selected) {
    this.resetForm();
    this.courseConflicts = [];
    this.courseOptions = [];
    this.resetConflictDiv();
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
    this.userService.getTeacherCourses(teacherId).subscribe((data: any) => {
      this.courses = data.courses;
      this.activities = data.activities;
      this.loadCourseSelect();
    }, error => {
      this.alertify.error(error);
    });
  }

  getCoursesByDay(classId) {
    this.classService.getScheduleCoursesByDay(classId).subscribe((data: any) => {
      this.coursesByDay = data;
    }, error => {
      this.alertify.error('problème pour récupérer les données');
    });
  }

  getScheduleCourses(classId) {
    this.resetSchedule();
    this.classService.getClassTimeTable(classId).subscribe((data: any) => {
      this.scheduleCourses = data.scheduleItems;
      // console.log('class schedule');
      // console.log(this.scheduleCourses);
      // data for existing courses select (conflict section)
      // for (let i = 0; i < this.scheduleCourses.days.length; i++) {
      //   const day = this.scheduleCourses.days[i];
      //   for (let j = 0; j < day.courses.length; j++) {
      //     const elt = day.courses[j];
      //     const oldCourse = {value: elt.id, label: elt.courseName.toUpperCase() + ' le ' + day.dayName + ' de ' + elt.startH +
      //       ' à ' + elt.endH + ' - ' + elt.teacherName.toUpperCase(), scheduleId: elt.scheduleId, day: day.day,
      //       dayName: day.dayName, color: elt.courseColor, teacher: elt.teacherName, startHM: elt.startH, endHM: elt.endH};
      //     this.oldCourseOptions = [...this.oldCourseOptions, oldCourse];
      //   }
      // }
      // add courses on the schedule
      for (let i = 1; i <= 7; i++) {
        const filtered = this.scheduleCourses.filter(items => items.day === i);
        for (let j = 0; j < filtered.length; j++) {
          switch (i) {
            case 1:
            this.monCourses.push(filtered[j]);
            break;
            case 2:
            this.tueCourses.push(filtered[j]);
            break;
            case 3:
            this.wedCourses.push(filtered[j]);
            break;
            case 4:
            this.thuCourses.push(filtered[j]);
            break;
            case 5:
            this.friCourses.push(filtered[j]);
            break;
            case 6:
            this.satCourses.push(filtered[j]);
            break;
            case 7:
            this.sunCourses.push(filtered[j]);
            break;
            default:
              break;
          }
        }
      }

      this.dayItems = [this.monCourses, this.tueCourses, this.wedCourses, this.thuCourses, this.friCourses, this.satCourses];
      // console.log(this.dayItems);
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

  toggleConflictDiv() {
    this.showConflictDiv = !this.showConflictDiv;
    if (this.showConflictDiv === false) {
      this.resetConflictDiv();
    }
  }

  resetConflictDiv() {
    this.conflictForm.get('oldCourse').setValue(null);
    this.conflictForm.get('conflictCourse').setValue(null);

    this.showConflictInfo = false;
    this.conflictDay = '';
    this.conflictStartHM = '';
    this.conflictEndHM = '';
    this.oldCourseColor = '';
    this.oldCourseInfo = 'N/D';
    this.conflictCourseColor = '';
    this.conflictCourseInfo = 'N/D';
}

  setConflictInfo(index, value) {
    this.showConflictInfo = true;
    // oldCourse changed?
    if (index === 0 && value) {
      const data = this.oldCourseOptions.find(c => c.value === value);
      if (data) {
        this.conflictDay = data.dayName;
        this.oldConflictStartHM = data.startHM;
        this.oldConflictEndHM = data.endHM;
        this.conflictStartHM = data.startHM;
        this.conflictEndHM = data.endHM;
        this.oldCourseColor = data.color;
        this.oldCourseInfo = data.label;
      }
    } else {
      // course to be added changed?
      const data = this.courseOptions.find(c => c.value === value);
      if (data) {
        this.conflictCourseColor = data.color;
        this.conflictCourseInfo = data.label.toUpperCase();
      }
    }
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
    this.courseOptions = [...this.courseOptions, {value: '', label: 'COURS', group: true}];
    for (let i = 0; i < this.courses.length; i++) {
      const elt = this.courses[i];
      this.courseOptions = [...this.courseOptions, {value: elt.id, label: elt.name, color: elt.color}];
    }

    if (this.activities.length > 0) {
      this.courseOptions = [...this.courseOptions, {value: '', label: 'ACTIVITES', group: true}];
      for (let i = 0; i < this.activities.length; i++) {
        const elt = this.activities[i];
        this.courseOptions = [...this.courseOptions, {value: elt.id, label: elt.name, color: elt.color}];
      }
    }
  }

  getClass(classid) {
    this.classService.getClass(classid).subscribe(data => {
      this.class = data;
      this.classEducLevel = this.class.classLevel.educationLevelId;
    }, error => {
      this.alertify.error(error);
    });
  }

  goBackToSchedule() {
  }

  editPeriod() {
    this.conflictForm.get('startHM').setValue(this.conflictStartHM);
    this.conflictForm.get('endHM').setValue(this.conflictEndHM);
    this.editPeriodDiv = true;
  }

  setPeriod() {
    this.editPeriodDiv = false;
    this.conflictStartHM = this.conflictForm.value.startHM;
    this.conflictEndHM = this.conflictForm.value.endHM;
    if (this.oldConflictStartHM !== this.conflictStartHM || this.oldConflictEndHM !== this.conflictEndHM) {
      this.periodChanged = true;
    } else {
      this.periodChanged = false;
    }
  }

  resetPeriod() {
    this.conflictStartHM = this.oldConflictStartHM;
    this.conflictEndHM = this.oldConflictEndHM;
    this.periodChanged = false;
  }

  saveScheduleItem() {
    this.scheduleItems = [];
    for (let i = 1; i <= 5; i++) {
      // is the schedule item line empty?
      if (this.scheduleForm.controls['item' + i].get('day' + i).value !== null) {
        // do we have a conflict for this line?
        const conflict = this.conflictData.find(c => c.line === i);
        const sch = <ScheduleData>{};
        if (conflict && conflict.line !== - 1) {
          sch.id = conflict.id;
          sch.scheduleId = conflict.scheduleId;
          sch.conflictedCourseId = conflict.conflictedCourseId;
        }
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
        this.scheduleItems = [...this.scheduleItems, sch];
      }
    }
    // console.log(this.scheduleItems);
    this.saveSchedule(this.scheduleItems);
  }

  saveSchedule(schedules: ScheduleData[]) {
    this.wait = true;
    this.classService.saveSchedules(schedules).subscribe(() => {
      const teacherId = this.scheduleForm.value.teacher;
      this.getTeacherSchedule(teacherId);
      this.getScheduleCourses(this.classId);
      this.resetForm();
      this.alertify.success('cours ajouté dans l\'emploi du temps');
      this.wait = false;
    }, error => {
      this.alertify.error('problème pour ajouter le cours dans l\'emploi du temps');
      this.wait = false;
    });
  }

  addCourseWithConflict() {
    const scheduleCourseId = this.conflictForm.value.oldCourse;
    const data = this.oldCourseOptions.find(c => c.value === scheduleCourseId);
    const startHM = this.conflictStartHM;
    const endHM = this.conflictEndHM;
    const conflictData = <any>{};
    if (this.periodChanged) {
      conflictData.conflictedCourseId = 0;
      conflictData.day = data.day;
      conflictData.startHourMin = startHM;
      conflictData.endHourMin = endHM;
    } else {
      conflictData.conflictedCourseId = scheduleCourseId;
      conflictData.scheduleId = data.scheduleId;
      conflictData.startHourMin = data.startHM;
      conflictData.endHourMin = data.endHM;
    }
    conflictData.classId = this.classId;
    conflictData.teacherId = this.scheduleForm.value.teacher;
    conflictData.CourseId = this.conflictForm.value.conflictCourse;
    this.classService.addCourseWithConflict(conflictData).subscribe(() => {
      const teacherId = this.scheduleForm.value.teacher;
      this.getTeacherSchedule(teacherId);
      this.getScheduleCourses(this.classId);
      this.resetForm();
      this.showConflictDiv = false;
      this.alertify.success('cours ajouté dans l\'emploi du temps');
    }, error => {
      this.alertify.error('problème pour saisir l\'emploi du temps');
    });
  }

}
