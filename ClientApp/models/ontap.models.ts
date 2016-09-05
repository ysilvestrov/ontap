export interface IElement {
    id: string;
}
export interface IBeer {
    id: string;
    name: string;
    description: string;
    type: string;
    brewery: string;
    alcohol: number;
    gravity: number;
    ibu: number;
}
export interface IPub {
    id: string;
    name: string;
    address: string;
    city: ICity;
    serves: IServe[];
}
export interface ICity {
    id: string;
    name: string;
}
export interface IServe {
    beer: IBeer;
    price: number;
}
export class Pub implements IPub {
    id: string;
    name: string;
    address: string;
    city: ICity;
    serves: IServe[];

    constructor(pub: IPub) {
        this.id = pub.id;
        this.name = pub.name;
        this.address = pub.address;
        this.city = pub.city;
        this.serves = pub.serves;
    }

}
export class City implements ICity {
    id: string;
    name: string;

    constructor(city: ICity) {
        this.id = city.id;
        this.name = city.name;
    }

}
export class Beer implements IBeer {
    id: string;
    name: string;
    description: string;
    type: string;
    brewery: string;
    alcohol: number;
    gravity: number;
    ibu: number;

    constructor(beer: IBeer) {
        this.id = beer.id;
        this.name = beer.name;
        this.description = beer.description;
        this.type = beer.type;
        this.brewery = beer.brewery;
        this.alcohol = beer.alcohol;
        this.gravity = beer.gravity;
        this.ibu = beer.ibu;
    }

}
