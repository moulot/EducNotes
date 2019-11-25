import { Periodicity } from './periodicity';
import { PayableAt } from './payable-at';

export class SchoolServicesDto {
  public id: number;

  public name: string;

  public comment: string;

  public price: number;

  public isByLevel: boolean;

  public isPeriodic: boolean;

  public periodicity: Periodicity;

  public payableAt: PayableAt;

  // constructor(
  //   id?: number,
  //   name?: string,
  //   comment?: string,
  //   price?: decimal,
  //   isByLevel?: boolean,
  //   isPeriodic?: boolean,
  //   periodicity?: Periodicity,
  //   payableAt?: PayableAt,
  // ) {
  //   this.id = id || 0;
  //   this.name = name || '';
  //   this.comment = comment || '';
  //   this.price = price || null;
  //   this.isByLevel = isByLevel || false;
  //   this.isPeriodic = isPeriodic || false;
  //   this.periodicity = periodicity || Periodicity;
  //   this.payableAt = payableAt || PayableAt;
  // }
}
