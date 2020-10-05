import { OrderLineDeadline } from './orderLineDeadline';

export interface OrderLine {
    id: number;
    orderId: number;
    orderLineLabel: string;
    deadline: Date;
    validity: Date;
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
    classLevelId: number;
    childClassId: number;
    childClassName: string;
    validated: boolean;
    paid: boolean;
    overDue: boolean;
    expired: boolean;
    cancelled: boolean;
    completed: boolean;
    active: boolean;
    insertDate: Date;
    insertUserId: number;
    updateDate: Date;
    updateUserId: number;
    payments: OrderLineDeadline[];
 }
