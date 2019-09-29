import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-grade-student',
  templateUrl: './grade-student.component.html',
  styleUrls: ['./grade-student.component.scss']
})
export class GradeStudentComponent implements OnInit {
  student: User;
  chartLineOption3: any;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      this.student = data['student'];
    });

    // this.chartLineOption3 = {
    //   ...echartStyles.lineNoAxis, ...{
    //     series: [{
    //       data: [40, 80, 20, 90, 30, 80, 40],
    //       lineStyle: {
    //         color: 'rgba(102, 51, 153, .86)',
    //         width: 3,
    //         shadowColor: 'rgba(0, 0, 0, .2)',
    //         shadowOffsetX: -1,
    //         shadowOffsetY: 8,
    //         shadowBlur: 10
    //       },
    //       label: { show: true, color: '#212121' },
    //       type: 'line',
    //       smooth: true,
    //       itemStyle: {
    //         borderColor: 'rgba(69, 86, 172, 0.86)'
    //       }
    //     }]
    //   }
    // };
    // this.chartLineOption3.xAxis.data = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

  }

}
