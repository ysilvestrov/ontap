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
    image: string;
}

export interface IPub {
    id: string;
    name: string;
    address: string;
    city: ICity;
    serves: IServe[];
    image: string;
    facebookUrl: string;
    vkontakteUrl: string;
    websiteUrl: string;
    bookingUrl: string;
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
    tap: number;
    volume: number;
    updated: Date;
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
    image: string;
    city: ICity;
    serves: IServe[];
    facebookUrl: string;
    vkontakteUrl: string;
    websiteUrl: string;
    bookingUrl: string;

    constructor(pub: IPub) {
        this.id = pub.id;
        this.name = pub.name;
        this.address = pub.address;
        this.city = pub.city;
        this.serves = pub.serves;
        this.image = pub.image;
        this.facebookUrl = pub.facebookUrl;
        this.vkontakteUrl = pub.vkontakteUrl;
        this.websiteUrl = pub.websiteUrl;
        this.bookingUrl = pub.bookingUrl;
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
    tap: number;
    volume: number;
    updated: Date;

    constructor(serve: IServe) {
        this.id = serve.id;
        this.served = serve.served;
        this.servedIn = serve.servedIn;
        this.price = serve.price;
        this.tap = serve.tap;
        this.volume = serve.volume;
        this.updated = serve.updated;
    }

}
export class Brewery implements IBrewery {
    constructor(brewery: IBrewery) {
        this.id = brewery.id;
        this.name = brewery.name;
        this.address = brewery.address;
        this.country = brewery.country;
        this.image = brewery.image;
    }

    id: string;
    name: string;
    address: string;
    country: ICountry;
    image: string;
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
    constructor(at: AccessToken) {
        this.accessToken = at.accessToken;
        this.expiresIn = at.expiresIn;
        this.expiresAt = at.expiresAt;
    }

    accessToken: string;
    expiresIn: number;
    expiresAt: Date;
}
export interface IPubAdmin {
    id: number;
    pub: IPub;
    user: IUser;
}
export class PubAdmin implements IPubAdmin {
    constructor(pa:IPubAdmin) {
        this.id = pa.id;
        this.pub = pa.pub;
        this.user = pa.user;
    }
    id: number;
    pub: IPub;
    user: IUser;
}
export interface IBreweryAdmin {
    id: number;
    brewery: IBrewery;
    user: IUser;
}
export class BreweryAdmin implements IBreweryAdmin {
    constructor(pa: IBreweryAdmin) {
        this.id = pa.id;
        this.brewery = pa.brewery;
        this.user = pa.user;
    }
    id: number;
    brewery: IBrewery;
    user: IUser;
}