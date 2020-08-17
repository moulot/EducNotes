import { Component, OnInit, Input, ElementRef, ViewChild, Renderer2 } from '@angular/core';
import { CardRotatingComponent } from 'ng-uikit-pro-standard';

@Component({
  selector: 'app-class-card',
  templateUrl: './class-card.component.html',
  styleUrls: ['./class-card.component.scss']
})
export class ClassCardComponent implements OnInit {
  @Input() level: any;

  constructor(private el: ElementRef, private renderer: Renderer2) { }
  @ViewChild('card', { static: true }) flippingCard: CardRotatingComponent;

  ngOnInit() {
    const cardWrapper = this.el.nativeElement.querySelectorAll('.card-wrapper');
    this.renderer.setStyle(cardWrapper[0], 'height', '200px');
  }

}
