package processes;

import core.Process;
import resources.TaskInSupervMemoryResource;

public class ReadFromKey extends Process{

    protected int priority = 95;

    public ReadFromKey(){
        this.pID = "ReadFromKey";
        this.priority = 95;
    }

    @Override
    public void execute() {
        switch(IC){
            case 0:
                //blokavimasis laukiant resurso "vartotojo sasaja"
                kernel.requestResource(this, "InputStreamResource");
                break;
            case 1:
                //programos nuskaitymas
                //blokavimasis laukiant resurso supervizorine atminties dalis is 100 zodziu?
                kernel.requestResource(this, "InputToSupervMemory");
                break;
            case 2:
                //eiluciu kopijavimas i supervizorine atminti
                //resurso superv atminties dalis is 100 zodziu atlaisvinimas
                kernel.freeResource(this, this.ownedResources.get(0));
                //sukuriamas ir atlaisvinamas resursas uzduotis supervizorineje atmintyje skirtas mainproc
                TaskInSupervMemoryResource r = new TaskInSupervMemoryResource();
                kernel.createResource(this, r);
                kernel.freeResource(this, r);
                break;
        }
    }
}