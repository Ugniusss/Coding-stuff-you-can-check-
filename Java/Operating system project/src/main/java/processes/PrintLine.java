package processes;

import core.Process;

public class PrintLine extends Process{

    protected int priority = 94;

    public PrintLine(){
        this.pID = "PrintLine";
        this.priority = 94;
    }

    @Override
    public void execute() {

    }
}