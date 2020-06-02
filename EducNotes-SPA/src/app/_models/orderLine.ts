import { OrderLineDeadline } from './orderLineDeadline';

export interface OrderLine {
   id: number;
   orderId: number;
   orderLineLabel: string;
   deadline: Date;
   strDeadline: string;
   productId: number;
   productName: string;
   qty: number;
   unitPrice: number;
   totalHT: number;
   discount: number;
   amountHT: number;
   tva: number;
   tvaAmount: number;
   amountTTC: number;
   strAmountTTC: string;
   childId: number;
   childFirstName: string;
   childLastName: string;
   childClassId: number;
   childClassName: string;
   cancelled: boolean;
   payments: OrderLineDeadline[];
}
