import { OrderlineToValidate } from './orderlineToValidate';

export interface OrderToValidate {
  orderId: number;
  orderlineIds: OrderlineToValidate[];
}
