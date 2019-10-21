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
  showDefaultChart = true;

  // LINE CHART
  chartType = 'line';
  chartDatasets = [
    { data: [65, 59, 80, 81, 56, 55, 40], label: 'My First dataset' },
    { data: [28, 48, 40, 19, 86, 27, 90], label: 'My Second dataset' }
  ];
  chartLabels = ['January', 'February', 'March', 'April', 'May', 'June', 'July'];
  chartColors: Array<any> = [
    {
      backgroundColor: 'rgba(255, 255, 255, .2)',
      borderColor: 'rgba(255, 0, 0, .7)',
      borderWidth: 2,
    },
    {
      backgroundColor: 'rgba(255, 255, 255, .2)',
      borderColor: 'rgba(0, 10, 130, .7)',
      borderWidth: 2,
    },
    {
      backgroundColor: 'rgba(255, 255, 255, .2)',
      borderColor: 'rgba(0, 255, 0, .7)',
      borderWidth: 2,
    }
  ];
  chartOptions: any = {
    responsive: true,
    plugins: {
      datalabels: {
        anchor: 'start',
        align: 'right',
        font: {
          size: 15,
        }
      }
    }
  };

  // RADAR CHART
  radarchartType = 'radar';
  radarchartDatasets = [
    { data: [65, 59, 80, 81, 56, 55, 40], label: 'My First dataset' },
    { data: [28, 48, 40, 19, 86, 27, 90], label: 'My Second dataset' }
  ];
  radarchartLabels = ['Eating', 'Drinking', 'Sleeping', 'Designing', 'Coding', 'Cycling', 'Running'];
  radarchartColors: Array<any>  = [
    {
      backgroundColor: 'rgba(255, 255, 255, .2)',
      borderColor: 'rgba(200, 99, 132, .7)',
      borderWidth: 2,
    },
    {
      backgroundColor: 'rgba(255, 255, 255, .2)',
      borderColor: 'rgba(0, 10, 130, .7)',
      borderWidth: 2,
    }
  ];
  radarchartOptions: any = {
    responsive: true
  };

  public chartClicked(e: any): void { }
  public chartHovered(e: any): void { }

  radarchartClicked(e: any): void { }
  radarchartHovered(e: any): void { }

  constructor(private route: ActivatedRoute, private evalService: EvaluationService,
    private alertify: AlertifyService, private classService: ClassService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.student = data['student'];
      this.getCoursesWithEvals(this.student.id, this.student.classId);
    });

  }

  loadData(course) {
    if (course) {
      this.showDefaultChart = false;
      this.chartDatasets = [
        { data: [], label: 'notes(-)' },
        { data: [], label: 'notes de ' + this.student.firstName},
        { data: [], label: 'notes(+)' }
      ];
      this.chartLabels = [];
      this.chartData = [];
      this.evalData = [];
      this.aboveAvg = [];
      const data = course.grades;

      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const ajustedGrade = (20 * elt.grade / elt.gradeMax).toFixed(2);
        const minGrade = (20 * elt.classGradeMin / elt.gradeMax).toFixed(2);
        const maxGrade = (20 * elt.classGradeMax / elt.gradeMax).toFixed(2);

        this.chartLabels = [...this.chartLabels, elt.evalDate];

        this.chartDatasets[0].data = [...this.chartDatasets[0].data, Number(minGrade)];
        this.chartDatasets[1].data = [...this.chartDatasets[1].data, Number(ajustedGrade)];
        this.chartDatasets[2].data = [...this.chartDatasets[2].data, Number(maxGrade)];

        this.chartData = [...this.chartData, Number(ajustedGrade)];
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
    this.chartLineOption3.xAxis.data = ['Mon', 'Tue', 'Wed', 'Thu'];
  }

  loadDefaultChart() {

    const data = this.userCourses;
    if (data.length > 0) {

      this.showDefaultChart = true;

      this.radarchartDatasets = [
        { data: [], label: 'notes de la classe' },
        { data: [], label: 'notes de ' + this.student.firstName}
      ];
      this.radarchartLabels = [];

      for (let i = 0; i < data.length; i++) {

        const elt = data[i];
        const userCourseGrade = elt.userCourseAvg;
        const classCourseGrade = elt.classCourseAvg;

        this.radarchartLabels = [...this.radarchartLabels, elt.courseName];

        this.radarchartDatasets[0].data = [...this.radarchartDatasets[0].data, Number(classCourseGrade)];
        this.radarchartDatasets[1].data = [...this.radarchartDatasets[1].data, Number(userCourseGrade)];
      }

    }
    console.log(this.radarchartDatasets);
    console.log(this.radarchartLabels);

  }

  getCoursesWithEvals(studentId, classId) {
    this.evalService.getUserCoursesWithEvals(classId, studentId).subscribe((data: any) => {
      this.userCourses = data.coursesWithEvals;
      this.studentAvg = data.studentAvg;
      this.loadDefaultChart();
    }, error => {
      this.alertify.error(error);
    });
  }

}
