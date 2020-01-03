/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { CommService } from './comm.service';

describe('Service: Comm', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CommService]
    });
  });

  it('should ...', inject([CommService], (service: CommService) => {
    expect(service).toBeTruthy();
  }));
});
