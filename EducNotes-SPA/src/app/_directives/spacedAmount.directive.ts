import { Directive, HostListener } from '@angular/core';
@Directive({
  selector: '[appSpacedAmnt]'
})
export class SpacedAmountDirective {

  constructor() { }

  @HostListener('input', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    const input = event.target as HTMLInputElement;

    const trimmed = input.value.replace(/\s+/g, '');

    const numbers = [];
    for (let i = 0; i < trimmed.length; i += 3) {
      numbers.push(trimmed.substr(i, 3));
    }

    input.value = numbers.join(' ');
  }
}
