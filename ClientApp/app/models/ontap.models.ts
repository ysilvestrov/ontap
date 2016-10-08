export interface IElement {
    id: any;
}
export interface IBeer {
    id: string;
    name: string;
    description: string;
    type: string;
    brewery: IBrewery;
    alcohol: number;
    gravity: number;
    ibu: number;
}
export interface IBrewery {
    id: string;
    name: string;
    address: string;
    country: ICountry;
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
export interface ICountry {
    id: string;
    name: string;
}
export interface IServe {
    id: number;
    served: IBeer;
    servedIn: IPub;
    price: number;
}
export interface IUser {
    id: string;
    name: string;
    password: string;
    isAdmin: boolean;
    canAdminPub: boolean;
    canAdminBrewery: boolean;
    pubs: IPub[];
    breweries: IBrewery[];
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
    brewery: IBrewery;
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
export class Serve implements IServe {
    id: number;
    served: IBeer;
    servedIn: IPub;
    price: number;

    constructor(serve: IServe) {
        this.id = serve.id;
        this.served = serve.served;
        this.servedIn = serve.servedIn;
        this.price = serve.price;
    }

}
export class Brewery implements IBrewery {
    constructor(brewery: IBrewery) {
        this.id = brewery.id;
        this.name = brewery.name;
        this.address = brewery.address;
        this.country = brewery.country;
    }

    id: string;
    name: string;
    address: string;
    country: ICountry;
}
export class User implements IUser {
    constructor(user :IUser) {
        this.id = user.id;
        this.name = user.name;
        this.password = user.password;
        this.isAdmin = user.isAdmin;
        this.canAdminPub = user.canAdminPub;
        this.canAdminBrewery = user.canAdminBrewery;
        this.pubs = user.pubs;
        this.breweries = user.breweries;
    }

    id: string;
    name: string;
    password: string;
    isAdmin: boolean;
    canAdminPub: boolean;
    canAdminBrewery: boolean;
    pubs: IPub[];
    breweries: IBrewery[];
}
export class AccessToken {
    accessToken: string;
    expiresIn: number;
    expiresAt: Date;
}