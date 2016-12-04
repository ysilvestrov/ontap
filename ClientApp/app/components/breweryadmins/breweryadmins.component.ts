import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import {IBrewery, IUser, IBreweryAdmin, BreweryAdmin} from "../../models/ontap.models";
import {BreweryService} from "../breweries/breweries.service";
import {UserService} from "../users/users.service";
import {BreweryAdminService} from "./breweryadmins.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";

@ng.Component({
    selector: 'breweryAdmins',
    providers: [BreweryAdminService, BreweryService, UserService],
  template: require('./breweryadmins.component.html')
})
export class BreweryAdminsComponent extends  AppComponent<IBreweryAdmin, BreweryAdminService> {
    public breweries: IBrewery[];
    public users: IUser[];

    constructor(elmService: BreweryAdminService, private breweryService: BreweryService, private userService: UserService, public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
        this.getBreweries();
        this.getUsers();
    }

    public startAdd() {
        super.startAdd();
        this.adding.brewery = new List(this.breweries).First();
        this.adding.user = new List(this.users).First();
    } 

    getBreweries() {
        this.breweryService.get()
            .subscribe(
            breweries => this.breweries = breweries,
            error => this.errorMessage = <any>error);
    }

    getUsers() {
        this.userService.get()
            .subscribe(
            users => this.users = users,
            error => this.errorMessage = <any>error);
    }

    onEditChangeBrewery(id) {
        this.onChangeBrewery(this.editing, id);
    }

    onAddChangeBrewery(id) {
        this.onChangeBrewery(this.adding, id);
    }

    onChangeBrewery(obj: IBreweryAdmin, id:string) {
        obj.brewery = new List(this.breweries).Where(c => c.id === id).First();
    }

    onEditChangeUser(id) {
        this.onChangeUser(this.editing, id);
    }

    onAddChangeUser(id) {
        this.onChangeUser(this.adding, id);
    }

    onChangeUser(obj: IBreweryAdmin, id:string) {
        obj.user = new List(this.users).Where(c => c.id === id).First();
    }
}