/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { TresoService } from './treso.service';

describe('Service: Treso', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TresoService]
    });
  });

  it('should ...', inject([TresoService], (service: TresoService) => {
    expect(service).toBeTruthy();
  }));
});
