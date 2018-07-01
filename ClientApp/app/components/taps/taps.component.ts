import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import {IPub, IUser, ITap, TapStatus} from "../../models/ontap.models";
import {EPubService} from "../epubs/epubs.service";
import {TapService} from "./taps.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";

@ng.Component({
    selector: 'taps',
    providers: [TapService, EPubService],
  template: require('./taps.component.html')
})
export class TapsComponent extends  AppComponent<ITap, TapService> {
    public pubs: IPub[];

// ReSharper disable InconsistentNaming
    public TapStatus_Problem = TapStatus.Problem;
    public TapStatus_Free = TapStatus.Free;
    public TapStatus_Working = TapStatus.Working;
// ReSharper restore InconsistentNaming

    constructor(elmService: TapService, private pubService: EPubService, public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
        this.getPubs();
    }

    public startAdd() {
        super.startAdd();
        this.adding.pub = new List(this.pubs).First();
    } 

    getPubs() {
        this.pubService.get()
            .subscribe(
            pubs => this.pubs = pubs,
            error => this.errorMessage = <any>error);
    }

    onEditChangePub(id) {
        this.onChangePub(this.editing, id);
    }

    onAddChangePub(id) {
        this.onChangePub(this.adding, id);
    }

    onChangePub(obj: ITap, id:string) {
        obj.pub = new List(this.pubs).Where(c => c.id === id).First();
    }
}