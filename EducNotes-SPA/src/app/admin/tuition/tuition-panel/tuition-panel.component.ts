import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { Setting } from 'src/app/_models/setting';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-tuition-panel',
  templateUrl: './tuition-panel.component.html',
  styleUrls: ['./tuition-panel.component.scss']
})
export class TuitionPanelComponent implements OnInit {
  settings: Setting[];
  regDate: Date;
  regActive = false;

  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.settings = this.authService.settings;
    const regDate = this.settings.find(s => s.name === 'RegistrationDate').value;
    this.regDate = Utils.inputDateDDMMYY(regDate, '/');
    const today = new Date();
    if (today >= this.regDate) {
      this.regActive = true;
    }
  }

}
