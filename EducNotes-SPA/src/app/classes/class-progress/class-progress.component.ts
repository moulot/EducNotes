import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-class-progress',
  templateUrl: './class-progress.component.html',
  styleUrls: ['./class-progress.component.scss']
})
export class ClassProgressComponent implements OnInit {
  program: any = [];
  currentClassId: number;

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private route: ActivatedRoute, private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      const programData = data['program'];
      this.authService.classId.subscribe(classId => this.currentClassId = classId);
      const index = programData.findIndex(c => c.classId === this.currentClassId);
      this.program = [...this.program, programData[index]][0];
    });
  }

}
