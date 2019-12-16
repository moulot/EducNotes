import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { PdfData } from 'src/app/_models/pdfData';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-convertToPDF',
  templateUrl: './convertToPDF.component.html',
  styleUrls: ['./convertToPDF.component.scss']
})
export class ConvertToPDFComponent implements OnInit {
valuei: any;
pdfData = <PdfData>{};
  constructor(private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  createPDF() {
   this.pdfData.html = '<h4>Hello Educ\'Notes - notes de GMK 1970</h4><ul><li>math : 12,4</li>' +
    '<li>français : 13,1</li><li>physique : 12</li></ul>';
   this.classService.getPDFFromHtml(this.pdfData).subscribe(() => {
    this.alertify.success('fichier PDF créé.');
   });
  }

}
