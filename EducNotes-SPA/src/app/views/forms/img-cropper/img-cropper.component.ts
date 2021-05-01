import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CropperSettings } from 'ngx-img-cropper';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-img-cropper',
  templateUrl: './img-cropper.component.html',
  styleUrls: ['./img-cropper.component.scss']
})
export class AppImgCropperComponent implements OnInit {
  data: any;
  cropperSettings: CropperSettings;
   @Output() imgResult = new EventEmitter();


  constructor(
    private modalService: NgbModal
  ) {
        this.cropperSettings = new CropperSettings();
        // this.cropperSettings.width = 100;
        // this.cropperSettings.height = 100;
        // this.cropperSettings.croppedWidth = 100;
        // this.cropperSettings.croppedHeight = 100;
        // this.cropperSettings.canvasWidth = 400;
        // this.cropperSettings.canvasHeight = 300;
        this.cropperSettings.cropperDrawSettings.lineDash = true;
        this.cropperSettings.cropperDrawSettings.dragIconStrokeWidth = 0;

        this.data = {};
  }

  ngOnInit() {
  }

  open(modal) {
    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title' })
    .result.then((result) => {
      console.log(result);
      this.imgResult.emit(this.data);
    }, (reason) => {
      console.log('Err!', reason);
    });
  }

}
