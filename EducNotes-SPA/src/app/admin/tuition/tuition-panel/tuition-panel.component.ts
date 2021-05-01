import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { Setting } from 'src/app/_models/setting';
import { Utils } from 'src/app/shared/utils';
import { OrderService } from 'src/app/_services/order.service';
import { environment } from 'src/environments/environment';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-tuition-panel',
  templateUrl: './tuition-panel.component.html',
  styleUrls: ['./tuition-panel.component.scss']
})
export class TuitionPanelComponent implements OnInit {
  teacherTypeId = environment.teacherTypeId;
  parentTypeId = environment.parentTypeId;
  studentTypeId = environment.studentTypeId;
  adminTypeId = environment.adminTypeId;
  settings: Setting[];
  regDate: Date;
  regActive = true;
  nbTuitionPays: number;
  tuitionBalance: string;
  admins: any;
  parents: any;
  teachers: any;
  students: any;
  totalTuitions: any;
  tuitionsNotValidated: any;
  tuitionsValidated: any;
  tuitionsByLevel: any;
  lastAdded: any;
  lastValidated: any;
  classSpaces: any;
  wait = false;
  dataReady = 0;
  studentpage = 1;
  studentpageSize = 5;

  // CHARTS
  chartType = 'bar';

  chartDatasets: Array<any> = [
    { data: [65, 59, -157, 81, 56, 55, 40], label: 'inscr. OK' },
    { data: [11, 12, 157, 13, 14, 15, 16], label: 'inscr. NOK' }
  ];

  chartLabels = [];

  chartColors = [
    {
      backgroundColor: [
        'rgba(255, 99, 132, 0.2)',
        'rgba(54, 162, 235, 0.2)',
        'rgba(255, 206, 86, 0.2)',
        'rgba(75, 192, 192, 0.2)',
        'rgba(153, 102, 255, 0.2)',
        'rgba(255, 159, 64, 0.2)'
      ],
      borderColor: [
        'rgba(255,99,132,1)',
        'rgba(54, 162, 235, 1)',
        'rgba(255, 206, 86, 1)',
        'rgba(75, 192, 192, 1)',
        'rgba(153, 102, 255, 1)',
        'rgba(255, 159, 64, 1)'
      ],
      borderWidth: 2,
    },
    {
      backgroundColor: [
        'rgba(255, 125, 158, 0.2)',
        'rgba(3, 111, 184, 0.2)',
        'rgba(255, 255, 137, 0.2)',
        'rgba(75, 192, 192, 0.2)',
        'rgba(126, 243, 243, 0.2)',
        'rgba(255, 210, 115, 0.2)'
      ],
      borderColor: [
        'rgba(255, 125, 158, 1)',
        'rgba(3, 111, 184, 1)',
        'rgba(255, 255, 137, 1)',
        'rgba(75, 192, 192, 1)',
        'rgba(126, 243, 243, 1)',
        'rgba(255, 210, 115, 1)'
      ],
      borderWidth: 2,
    },
  ];

  public chartOptions: any = {
      responsive: true,
        scales: {
          xAxes: [{
            stacked: true
            }],
          yAxes: [
          {
            stacked: true
          }
        ]
      }
  };

  chartPctType = 'polarArea'; // horizontalBar
  chartPctDatasets: Array<any> = [
    { data: [65, 59, 80, 81, 56, 55, 40], label: 'taux inscriptions (%)' }
  ];
  // chartPctLabels: Array<any> = ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'];
  chartPctColors: Array<any> = [
    {
      backgroundColor: [
        'rgba(255, 99, 132, 0.2)',
        'rgba(54, 162, 235, 0.2)',
        'rgba(255, 206, 86, 0.2)',
        'rgba(75, 192, 192, 0.2)',
        'rgba(153, 102, 255, 0.2)',
        'rgba(255, 159, 64, 0.2)'
      ],
      borderColor: [
        'rgba(255,99,132,1)',
        'rgba(54, 162, 235, 1)',
        'rgba(255, 206, 86, 1)',
        'rgba(75, 192, 192, 1)',
        'rgba(153, 102, 255, 1)',
        'rgba(255, 159, 64, 1)'
      ],
      borderWidth: 2,
    }
  ];
  public chartPctOptions: any = {
    responsive: true
  };

  constructor(private authService: AuthService, private orderService: OrderService,
    private alertify: AlertifyService, private userService: UserService) { }

  ngOnInit() {
    this.getUSersRecap();
    this.getTuitionFigures();
    this.getTuitionsByLevel();
    this.getLastTuitions();
    this.settings = this.authService.settings;
    const regDate = this.settings.find(s => s.name === 'RegistrationDate').value;
    this.regDate = Utils.inputDateDDMMYY(regDate, '/');
    this.orderService.getBalanceData().subscribe((data: any) => {
      this.tuitionBalance = data.openBalance;
    });
  }

  getUSersRecap() {
    this.wait = true;
    this.userService.getUsersRecap().subscribe((data: any) => {
      this.dataReady++;
      if (this.dataReady === 4) {
        this.wait = false;
      }
      this.admins = data.find(i => i.userTypeId === this.adminTypeId);
      this.parents = data.find(i => i.userTypeId === this.parentTypeId);
      this.teachers = data.find(i => i.userTypeId === this.teacherTypeId);
      this.students = data.find(i => i.userTypeId === this.studentTypeId);
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  getTuitionFigures() {
    this.wait = true;
    this.orderService.getTuitionFigures().subscribe((data: any) => {
      this.dataReady++;
      if (this.dataReady === 4) {
        this.wait = false;
      }
      this.totalTuitions = data.totalTuitions;
      this.tuitionsNotValidated = data.tuitionsNotValidated;
      this.tuitionsValidated = data.tuitionsValidated;
      this.classSpaces = data.classSpaces;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  getTuitionsByLevel() {
    this.wait = true;
    this.orderService.getTuitionList().subscribe((data: any) => {
      this.dataReady++;
      if (this.dataReady === 4) {
        this.wait = false;
      }
      this.tuitionsByLevel = data.tuitionList;
      this.setChartsData();
    }, () => {
      this.alertify.error('problème de récupération des données');
      this.wait = false;
    });
  }

  getLastTuitions() {
    this.wait = true;
    this.orderService.getLastTuitions().subscribe((data: any) => {
      this.dataReady++;
      if (this.dataReady === 4) {
        this.wait = false;
      }
      this.lastAdded = data.lastAdded;
      this.lastValidated = data.lastValidated;
    }, () => {
      this.alertify.error('problème de récupération des données');
      this.wait = false;
    });
  }

  setChartsData() {
    let tuitionsOK = this.chartDatasets[0].data = [];
    let tuitionsNOK = this.chartDatasets[1].data = [];
    let pctTuitionsOK = this.chartPctDatasets[0].data = [];
    for (let i = 0; i < this.tuitionsByLevel.length; i++) {
      const elt = this.tuitionsByLevel[i];
      this.chartLabels = [...this.chartLabels, elt.classLevelName];
      tuitionsOK = [...tuitionsOK, elt.nbTuitionsOK];
      tuitionsNOK = [...tuitionsNOK, elt.nbTuitions];
      pctTuitionsOK = [...pctTuitionsOK, elt.pctValidatedOfMax];
    }
    this.chartDatasets[0].data = tuitionsOK;
    this.chartDatasets[1].data = tuitionsNOK;
    this.chartPctDatasets[0].data = pctTuitionsOK;
  }

  counter(i: number) {
    return new Array(i);
  }

  public chartClicked(e: any): void { }
  public chartHovered(e: any): void { }

  public chartPctClicked(e: any): void { }
  public chartPctHovered(e: any): void { }

}
