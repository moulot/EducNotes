import { Class } from './class';
import { Inscription } from './inscription';

export interface ClassLevel {
    id: number;
    name: string;
    dsplSeq: number;
    Inscriptions: Inscription[];
    classes: Class[];
}
