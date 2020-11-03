import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { echartStyles } from 'src/app/shared/echart-styles';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

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
  periodAvgs: any;
  chartData: number[] = [];
  evalData: any[] = [];
  aboveAvg: boolean[] = [];
  showDefaultChart = true;
  isParentConnected = false;
  periods: any;
  btnColor: any[] = [];
  selectedPeriod = true;
  selectedCourse = false;
  coursesAvgs: any[] = [];
  btnDisabled: any[] = [];
  userIdFromRoute: any;
  showChildrenList = false;
  parent: User;
  children: User[];
  periodAvg: number;
  periodName = '';
  url = '/studentGradesP';

  headElts = ['date', 'type', 'note', '(-)', '(+)'];

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
    plugins: [ChartDataLabels],
    options: {
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
    private alertify: AlertifyService, private classService: ClassService,
    private authService: AuthService, private userService: UserService) { }

  ngOnInit() {
    // this.route.data.subscribe(data => {
    //   this.student = data['student'];
    //   this.getUserGrades(this.student.id, this.student.classId);
    // });
    this.route.params.subscribe(params => {
      this.userIdFromRoute = params['id'];
    });

    // is the parent connected?
    if (Number(this.userIdFromRoute) === 0) {
      this.showChildrenList = true;
      this.parent = this.authService.currentUser;
      this.getChildren(this.parent.id);
    } else {
      this.showChildrenList = false;
      this.getUser(this.userIdFromRoute);
    }

  }

  getUser(id) {
    this.userService.getUser(id).subscribe((user: User) => {
    this.student = user;

    const loggedUser = this.authService.currentUser;
    if (loggedUser.id !== this.student.id) {
      this.isParentConnected = true;
    }

   this.getUserGrades(this.student.id, this.student.classId);
    this.showChildrenList = false;
  }, error => {
      this.alertify.error(error);
    });
  }

  getChildren(parentId: number) {
    this.userService.getChildren(parentId).subscribe((users: User[]) => {
      this.children = users;
    }, error => {
      this.alertify.error(error);
    });
  }

  loadData(course) {
    if (course) {
      this.selectedCourse = true;
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

    const nb = this.btnColor.length - 1;
    this.btnColor = [];
    this.btnColor = [...this.btnColor, 'primary'];
    for (let i = 0; i < nb; i++) {
      this.btnColor = [...this.btnColor, 'light'];
    }

    this.selectedCourse = false;
    this.selectedPeriod = false;

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
        if (Number(userCourseGrade) !== -1000) { // -1000 (in the API) for a course that doesn't have grades
          this.radarchartLabels = [...this.radarchartLabels, elt.courseName];

          this.radarchartDatasets[0].data = [...this.radarchartDatasets[0].data, Number(classCourseGrade)];
          this.radarchartDatasets[1].data = [...this.radarchartDatasets[1].data, Number(userCourseGrade)];
        }
      }

    }

  }

  loadChartSetBtn(periodId: number, index: number) {
    const nb = this.btnColor.length - 1;
    this.btnColor = [];
    this.btnColor = [...this.btnColor, 'light'];
    for (let i = 0; i < nb; i++) {
      const elt = i + 1 === index ? 'primary' : 'light';
      this.btnColor = [...this.btnColor, elt];
    }

    this.loadPeriodChart(periodId);
  }

  loadPeriodChart(periodId: number) {

    if (periodId > 0) {
      // is a period selected?
      this.selectedPeriod = true;
      // get the selected period name & avg
      const periodData = this.periodAvgs.find(item => item.periodId === periodId);
      this.periodName = periodData.periodName;
      this.periodAvg = periodData.avg;
    } else {
      // get the active (default) period name & avg
      const periodData = this.periodAvgs.find(item => item.active === true);
      this.periodName = periodData.periodName;
      this.periodAvg = periodData.avg;
    }

    this.selectedCourse = false;
    this.coursesAvgs = [];
    const data = this.userCourses;

    if (data.length > 0) {

      this.showDefaultChart = true;

      this.radarchartDatasets = [
        { data: [], label: 'notes de la classe' },
        { data: [], label: 'notes de ' + this.student.firstName}
      ];
      this.radarchartLabels = [];

      for (let i = 0; i < data.length; i++) {

        const course = data[i];
        let elt: any;
        if (periodId === 0) {
          elt = course.periodEvals.find(item => item.active === true);
        } else {
          elt = course.periodEvals.find(item => item.periodId === periodId);
        }
        const userCourseGrade = elt.userCourseAvg;
        const classCourseGrade = elt.classCourseAvg;
        if (Number(userCourseGrade) !== -1000) { // -1000 (in the API) for a course that doesn't have grades
          this.radarchartLabels = [...this.radarchartLabels, course.courseName];

          this.radarchartDatasets[0].data = [...this.radarchartDatasets[0].data, Number(classCourseGrade)];
          this.radarchartDatasets[1].data = [...this.radarchartDatasets[1].data, Number(userCourseGrade)];
        }

        // set the course Averages
        const avg = {abbrev: course.courseAbbrev, avg: elt.userCourseAvg, grades: elt.grades};
        this.coursesAvgs = [...this.coursesAvgs, avg];
      }

    }

  }

  getUserGrades(studentId, classId) {
    this.evalService.getUserCoursesWithEvals(classId, studentId).subscribe((data: any) => {

      this.userCourses = data.coursesWithEvals;
      this.studentAvg = data.studentAvg;
      this.periodAvgs = data.periodAvgs;
      this.periods = data.periods;

      this.btnColor = [];
      this.btnColor = [...this.btnColor, 'light'];
      for (let i = 0; i < this.periodAvgs.length; i++) {
        const elt = this.periodAvgs[i];
        // set the color period buttons
        const color = elt.active === true ? 'primary' : 'light';
        this.btnColor = [...this.btnColor, color];
        // disable period buttons without data. done with startDate of period
        let disabled = true;
        if (elt.activated === true) {
          disabled = false;
        }
        this.btnDisabled = [...this.btnDisabled, disabled];
      }

      this.loadPeriodChart(0); // 0 for current period
    }, error => {
      this.alertify.error(error);
    });
  }

}
