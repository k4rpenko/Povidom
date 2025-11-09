import { Component } from '@angular/core';
import {IndexDBComponent} from "../../api/UserDB/index-db/index-db.component";

@Component({
  selector: 'app-premium',
    imports: [
        IndexDBComponent
    ],
  templateUrl: './premium.component.html',
  styleUrl: './premium.component.scss'
})
export class PremiumComponent {

}
