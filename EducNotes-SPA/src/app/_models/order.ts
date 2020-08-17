import { OrderLine } from './orderLine';

export interface Order {
  id: number;
  orderTypeId: number;
  orderNum: number;
  orderLabel: string;
  orderDate: Date;
  strOrderDate: string;
  deadline: Date;
  strDeadline: string;
  validity: Date;
  strValidity: string;
  shippingAddressId?: number;
  billingAddressId?: number;
  totalHT: number;
  discount: number;
  amountHT: number;
  tva: number;
  tvaAmount: number;
  amountTTC: number;
  strAmountTTC: number;
  childId: number;
  childLastName: string;
  childFirstName: string;
  childClassId: number;
  childClassName: string;
  parentId: number;
  parentLastName: string;
  parentFirstName: string;
  parentCell: string;
  parentEmail: string;
  status: number;
  isReg: boolean;
  isNextReg: boolean;
  lines: OrderLine[];
}
