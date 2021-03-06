import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import {IPub, IUser, IPubAdmin, PubAdmin} from "../../models/ontap.models";
import {EPubService} from "../epubs/epubs.service";
import {UserService} from "../users/users.service";
import {PubAdminService} from "./pubadmins.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";

@ng.Component({
    selector: 'pubAdmins',
    providers: [PubAdminService, EPubService, UserService],
  template: require('./pubadmins.component.html')
})
export class PubAdminsComponent extends  AppComponent<IPubAdmin, PubAdminService> {
    public pubs: IPub[];
    public users: IUser[];

    constructor(elmService: PubAdminService, private pubService: EPubService, private userService: UserService, public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
        this.getPubs();
        this.getUsers();
    }

    public startAdd() {
        super.startAdd();
        this.adding.pub = new List(this.pubs).First();
        this.adding.user = new List(this.users).First();
    } 

    getPubs() {
        this.pubService.get()
            .subscribe(
            pubs => this.pubs = pubs,
            error => this.errorMessage = <any>error);
    }

    getUsers() {
        this.userService.get()
            .subscribe(
            users => this.users = users,
            error => this.errorMessage = <any>error);
    }

    onEditChangePub(id) {
        this.onChangePub(this.editing, id);
    }

    onAddChangePub(id) {
        this.onChangePub(this.adding, id);
    }

    onChangePub(obj: IPubAdmin, id:string) {
        obj.pub = new List(this.pubs).Where(c => c.id === id).First();
    }

    onEditChangeUser(id) {
        this.onChangeUser(this.editing, id);
    }

    onAddChangeUser(id) {
        this.onChangeUser(this.adding, id);
    }

    onChangeUser(obj: IPubAdmin, id:string) {
        obj.user = new List(this.users).Where(c => c.id === id).First();
    }
}