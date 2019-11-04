export class PayableAt {
  public id: number;

  public name: string;

  public dayCount: number;

  constructor(
    id?: number,
    name?: string,
    dayCount?: number,
    ) {
    this.id = id || 0;
    this.name = name || '';
    this.dayCount = dayCount || 0;
  }
}
