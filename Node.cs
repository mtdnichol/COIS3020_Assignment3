using System.Linq;

namespace COIS3020_Assignment3 {
    public class Node {
        private Color color; //Either coloured black, white, or grey.  Black/White are leaf nodes
        private Node nw, ne, se, sw; //Four nodes representing four quadrants of grey nodes

        public Node() { //Default constructor
            this.color = Color.GRAY;
        }

        public Node(Color color) {
            this.color = color;
        }

        // Public LikeChildren
        // Checks if each child referenced by this node is the same color
        // Returns: boolean variable representing result of comparison
        public bool LikeChildren() {
            return nw.getColor().Equals(ne.getColor()) && nw.getColor().Equals(se.getColor()) && nw.getColor().Equals(sw.getColor());
        }

        public void ClearChildren() {
            this.nw = null;
            this.ne = null;
            this.se = null;
            this.sw = null;
        }
 
        //Access properties GET/SET
        public Color getColor() { return this.color; }
        public void setColor(Color color) { this.color = color; }

        public Node getNW() { return this.nw; }
        public Node getNE() { return this.ne; }
        public Node getSE() { return this.se; }
        public Node getSW() { return this.sw; }
        
        public void setNW(Node node) { this.nw = node; }
        public void setNE(Node node) { this.ne = node; }
        public void setSE(Node node) { this.se = node; }
        public void setSW(Node node) { this.sw = node; }
    }
}