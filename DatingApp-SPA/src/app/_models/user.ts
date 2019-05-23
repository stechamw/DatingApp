import { Photo } from './photo';

export interface User {
    id: number;
    username: string;
    knownAs: string;
    age: number;
    gender: string;
    created: Date;
    lastActive: Date;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string; /** ? question mark makes a property optional known as elvis operator */
    introduction?: string;
    lookingFor?: string;
    photo?: Photo[]; /** type photo of an array create interface for photos as well */

}
