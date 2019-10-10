import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { echartStyles } from 'src/app/shared/echart-styles';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Class } from 'src/app/_models/class';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-grade-student',
  templateUrl: './grade-student.component.html',
  styleUrls: ['./grade-student.component.scss']
})
export class GradeStudentComponent implements OnInit {
  student: User;
  chartLineOption3: any;
  userCourses: any;
  studentAvg: any;
  classRoom: Class;
  chartData: number[] = [];
  evalData: any[] = [];
  aboveAvg: boolean[] = [];

  constructor(private route: ActivatedRoute, private evalService: EvaluationService,
    private alertify: AlertifyService, private classService: ClassService) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      this.student = data['student'];
      this.getCoursesWithEvals(this.student.id, this.student.classId);
      this.getClass(this.student.classId);
    });

  }

  loadData(course) {

    if (course) {
      this.chartData = [];
      this.evalData = [];
      this.aboveAvg = [];
      const data = course.grades;

      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const ajustedGrade = 20 * elt.grade / elt.gradeMax;
        this.chartData = [...this.chartData, ajustedGrade];

        this.evalData = [...this.evalData, elt];

        const g = elt.grade;
        const avg = elt.gradeMax / 2;

        let isAbove: boolean;
        if (Number(g) < Number(avg)) {
          isAbove = false;
        } else {
          isAbove = true;
        }
        this.aboveAvg = [...this.aboveAvg, isAbove];
      }

      this.loadChart();
    }

  }

  loadChart() {
    this.chartLineOption3 = {
      ...echartStyles.lineNoAxis, ...{
        series: [{
          data: this.chartData, // [13, 10, 9, 14, 10, 13, 18],
          lineStyle: {
            color: 'rgba(102, 51, 153, .86)',
            width: 3,
            shadowColor: 'rgba(0, 0, 0, .2)',
            shadowOffsetX: -1,
            shadowOffsetY: 8,
            shadowBlur: 10
          },
          label: { show: true, color: '#212121' },
          type: 'line',
          smooth: true,
          itemStyle: {
            borderColor: 'rgba(69, 86, 172, 0.86)'
          }
        }]
      }
    };
    this.chartLineOption3.xAxis.data = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
  }

  getCoursesWithEvals(studentId, classId) {
    this.evalService.getUserCoursesWithEvals(classId, studentId).subscribe((data: any) => {
      this.userCourses = data.coursesWithEvals;
      this.studentAvg = data.studentAvg;
    }, error => {
      this.alertify.error(error);
    });
  }

  getClass(classId) {
    this.classService.getClass(classId).subscribe(data => {
      this.classRoom = data;
    }, error => {
      this.alertify.error(error);
    });
  }

}
