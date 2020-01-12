import { Component, OnInit, Input, Renderer2, ElementRef } from '@angular/core';

@Component({
  selector: 'app-sms-template-card',
  templateUrl: './sms-template-card.component.html',
  styleUrls: ['./sms-template-card.component.scss']
})
export class SmsTemplateCardComponent implements OnInit {
  @Input() template: any;

  constructor(private renderer: Renderer2, private el: ElementRef) { }

  ngOnInit() {
    const cardWrapper = this.el.nativeElement.querySelectorAll('.card-wrapper');
    this.renderer.setStyle(cardWrapper[0], 'height', '275px');
  }

}
