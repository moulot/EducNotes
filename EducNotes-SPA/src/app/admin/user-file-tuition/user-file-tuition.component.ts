import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-user-file-tuition',
  templateUrl: './user-file-tuition.component.html',
  styleUrls: ['./user-file-tuition.component.scss']
})
export class UserFileTuitionComponent implements OnInit {
  @Input() id: any;
  tuition: any;

  constructor( private route: ActivatedRoute, private alertify: AlertifyService,
    private userService: UserService) { }

  ngOnInit() {
    this.getTuition(this.id);
  }

  getTuition(id) {
    this.userService.getTuition(id).subscribe(data => {
      this.tuition = data;
    }, error => {
      this.alertify.error(error);
    });
  }

}
