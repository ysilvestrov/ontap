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
    bjcpStyle: string;
    image: string;
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
    taplistHeaderImage: string;
    taplistFooterImage: string;
    facebookUrl: string;
    vkontakteUrl: string;
    websiteUrl: string;
    bookingUrl: string;
    parserOptions: string;
    tapNumber: number;
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
    taplistHeaderImage: string;
    taplistFooterImage: string;
    city: ICity;
    serves: IServe[];
    facebookUrl: string;
    vkontakteUrl: string;
    websiteUrl: string;
    bookingUrl: string;
    parserOptions: string;
    tapNumber: number;

    public constructor(init?: Partial<IPub>) {
        Object.assign(this, init);
    }
}
export class City implements ICity {
    id: string;
    name: string;

    public constructor(init?: Partial<ICity>) {
        Object.assign(this, init);
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
    bjcpStyle: string;
    image: string;

    public constructor(init?: Partial<IBeer>) {
        Object.assign(this, init);
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

    public constructor(init?: Partial<IServe>) {
        Object.assign(this, init);
    }

}
export class Brewery implements IBrewery {
    id: string;
    name: string;
    address: string;
    country: ICountry;
    image: string;

    public constructor(init?: Partial<IBrewery>) {
        Object.assign(this, init);
    }
}
export class User implements IUser {
    id: string;
    name: string;
    password: string;
    isAdmin: boolean;
    canAdminPub: boolean;
    canAdminBrewery: boolean;
    pubs: IPub[];
    breweries: IBrewery[];

    public constructor(init?: Partial<IUser>) {
        Object.assign(this, init);
    }
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
    id: number;
    pub: IPub;
    user: IUser;

    public constructor(init?: Partial<IPubAdmin>) {
        Object.assign(this, init);
    }
}
export interface IBreweryAdmin {
    id: number;
    brewery: IBrewery;
    user: IUser;
}
export class BreweryAdmin implements IBreweryAdmin {
    id: number;
    brewery: IBrewery;
    user: IUser;

    public constructor(init?: Partial<IBreweryAdmin>) {
        Object.assign(this, init);
    }
}